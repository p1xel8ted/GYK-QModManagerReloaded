using Mono.Cecil;
using Mono.Cecil.Cil;
using QModReloaded;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using File = System.IO.File;

namespace QModReloadedGUI;

public partial class FrmMain : Form
{
    private static readonly string[] CleanMd5Hashes = {
        "e5c55499ebbf010e341f0f56e12f6c74", "b75466bdcc44f5f098d4b22dc047b175"
    };

    //hash for Assembly-CSharp.dll 1.405
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
    };

    private static string _path = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\"));
    private readonly List<QMod> _modList = new();
    private Rectangle _dragBoxFromMouseDown;
    private FrmAbout _frmAbout;
    private FrmChecklist _frmChecklist;
    private FrmConfigEdit _frmConfigEdit;
    private FrmResModifier _frmResModifier;
    private (string location, bool found) _gameLocation;
    private Injector _injector;
    private string _modLocation = string.Empty;
    private int _rowIndexFromMouseDown;
    private int _rowIndexOfItemUnderMouseToDrop;

    public FrmMain()
    {
        InitializeComponent();
    }

    private static bool CreateJson(string file)
    {
        var sFile = new FileInfo(file);
        var path = new FileInfo(file).DirectoryName;
        var fileNameWithoutExt = sFile.Name.Substring(0, sFile.Name.Length - 4);
        var fileNameWithExt = sFile.Name;
        var (namesp, type, method, found) = GetModEntryPoint(file);
        var modInfo = FileVersionInfo.GetVersionInfo(file);
        var configInfo = GetModConfigIfItExists(path);
        var config = string.Empty;
        if (configInfo.exists)
        {
            config = Path.GetFileName(configInfo.file);
        }
        var newMod = new QMod
        {
            DisplayName = modInfo.ProductName,
            Enable = true,
            ModAssemblyPath = path,
            AssemblyName = fileNameWithExt,
            Author = modInfo.CompanyName,
            Description = modInfo.FileDescription,
            Config = config,
            Id = fileNameWithoutExt,
            EntryMethod = found ? $"{namesp}.{type}.{method}" : $"{fileNameWithoutExt}.MainPatcher.Patch",
            Version = modInfo.FileVersion
        };
        var newJson = JsonSerializer.Serialize(newMod, JsonOptions);
        if (path == null) return false;
        File.WriteAllText(Path.Combine(path, "mod.json"), newJson);
        var files = new FileInfo(Path.Combine(path, "mod.json"));
        return files.Exists;
    }

    private static (bool exists, string file) GetModConfigIfItExists(string modFilePath)
    {
        string path = null;
        var files = Directory.GetFiles(modFilePath, "*", SearchOption.AllDirectories);
        string[] configs = { ".ini", ".json", ".txt", ".cfg" };
        foreach (var file in files)
        {
            if (file.EndsWith("mod.json")) continue;
            if (!file.Contains("config")) continue;
            if (configs.Contains(new FileInfo(file).Extension))
            {
                path = file;
            }
        }

        if (path == null) return (false, null);
        Console.WriteLine($@"Found config: {path}");
        return !File.Exists(path) ? (false, null) : (true, path);
    }

    private static (string namesp, string type, string method, bool found) GetModEntryPoint(string mod)
    {
        try
        {
            var modAssembly = AssemblyDefinition.ReadAssembly(mod);

            var toInspect = modAssembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods
                    .Where(m => m.HasBody)
                    .Select(m => new { t, m }));

            // toInspect = toInspect.Where(x => x.m.Name == "Patch");

            foreach (var method in toInspect)
                if (method.m.Body.Instructions.Where(instruction => instruction.Operand != null)
                    .Any(instruction => instruction.Operand.ToString().Contains("PatchAll")))
                {
                    Logger.WriteLog($"{method.t.Namespace}.{method.t.Name}.{method.m.Name}");
                    return (method.t.Namespace, method.t.Name, method.m.Name, true);
                }
        }
        catch (Exception ex)
        {
            Logger.WriteLog($"GetModEntryPoint(): Error, {ex.Message}");
        }

        return (null, null, null, false);
    }

    private static bool IsGameRunning()
    {
        var processes = Process.GetProcessesByName("Graveyard Keeper");
        if (processes.Length <= 0) return false;
        MessageBox.Show(@"Please close the game before running any patches.", @"Close game.",
            MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation);
        return true;
    }

    private static bool IsModCompatible(string mod)
    {
        try
        {
            var modAssembly = AssemblyDefinition.ReadAssembly(mod);

            var toInspect = modAssembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods
                    .Where(m => m.HasBody)
                    .Select(m => new { t, m }));

            //toInspect = toInspect.Where(x => x.m.Name is "Patch");

            if (toInspect.Any(method => method.m.Body.Instructions.Where(instruction => instruction.Operand != null)
                    .Any(instruction => instruction.OpCode == OpCodes.Newobj && instruction.Operand.ToString().Contains("HarmonyLib.Harmony"))))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLog($"IsModCompatible(): Error, {ex.Message}");
        }

        return false;
    }

    private static bool IsSteamCopy()
    {
        return Directory.GetFiles(_path).Select(file => new FileInfo(file)).Any(sFile => sFile.Name.Contains("steam"));
    }

    private void AboutToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        _frmAbout ??= new FrmAbout();
        _frmAbout.ShowDialog();
        _frmAbout = null;
    }

    private bool AddMod(string file)
    {
        try
        {
            var modZip = new FileInfo(file);
            if (!modZip.Exists) return false;
            var modArchive = ZipFile.OpenRead(modZip.FullName);
            foreach (var entry in modArchive.Entries)
            {
                if (entry.FullName.EndsWith("dll", StringComparison.OrdinalIgnoreCase))
                {
                    ZipFile.ExtractToDirectory(file,
                        _modLocation + "\\" + entry.FullName.Substring(0, entry.FullName.Length - 4));
                    break;
                }

                ZipFile.ExtractToDirectory(file, _modLocation);
                break;
            }

            return true;
        }
        catch (Exception ex)
        {
            WriteLog($"ZIP Module: {ex.Message}", true);
            return false;
        }
    }

    private void BtnAddMod_Click(object sender, EventArgs e)
    {
        var dlgResult = DlgFile.ShowDialog(this);
        if (dlgResult == DialogResult.OK)
            foreach (var zip in DlgFile.FileNames)
            {
                var result = AddMod(zip);
                if (result)
                {
                    WriteLog($"Extracted {zip}.");
                }
                else
                {
                    WriteLog($"Issue extracting {zip}.", true);
                }
            }

        LoadMods();
    }

    private void BtnLaunchModless_Click(object sender, EventArgs e)
    {
        if (!_gameLocation.found) return;
        if (IsGameRunning()) return;
        foreach (var mod in _modList)
        {
            WriteLog("Disabling mods and launching game.");
            mod.Enable = false;
            var newJson = JsonSerializer.Serialize(mod, JsonOptions);
            File.WriteAllText(Path.Combine(mod.ModAssemblyPath, "mod.json"), newJson);
        }
        RunGame();
    }

    private void BtnOpenGameDir_Click(object sender, EventArgs e)
    {
        if (_gameLocation.found)
            Process.Start(_gameLocation.location);
        else
            MessageBox.Show(@"Set game location first.", @"Game", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
    }

    private void BtnOpenLog_Click(object sender, EventArgs e)
    {
        var file = Path.Combine(_gameLocation.location, "qmod_reloaded_log.txt");
        if (File.Exists(file))
            Process.Start(file);
        else
            MessageBox.Show(@"No log available yet.", @"Log", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
    }

    private void BtnOpenModDir_Click(object sender, EventArgs e)
    {
        if (_gameLocation.found)
            Process.Start(_modLocation);
        else
            MessageBox.Show(@"Set game location first.", @"Game", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
    }

    private void BtnPatch_Click(object sender, EventArgs e)
    {
        if (IsGameRunning()) return;
        if (_injector.IsInjected())
        {
            MessageBox.Show(@"All patching already done!", @"Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var (_, message) = _injector.Inject();
        WriteLog(message);
        CheckPatched();
    }

    private void BtnRefresh_Click(object sender, EventArgs e)
    {
        if (_modList.Count <= 0)
        {
            MessageBox.Show("No mods installed to refresh!", "Umm", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }
        UpdateModJsons();
        LoadMods();
    }

    private void BtnRemove_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(@"This will remove the selected mod(s). Continue?", @"Remove mods",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;
        foreach (DataGridViewRow rows in DgvMods.SelectedRows)
        {
            var modId = rows.Cells[7].Value.ToString();
            var mod = FindMod(modId);
            if (mod == null) return;
            Directory.Delete(mod.ModAssemblyPath, true);
            _modList.Remove(mod);
        }

        LoadMods();
    }

    private void BtnRemoveIntros_Click(object sender, EventArgs e)
    {
        if (IsGameRunning()) return;
        if (_injector.IsNoIntroInjected() && !_injector.IsInjected())
        {
            var alreadyResult = MessageBox.Show(@"Intro patch already done! Apply mod patch now?", @"Done!",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (alreadyResult == DialogResult.Yes) BtnPatch_Click(sender, e);
            return;
        }

        if (_injector.IsNoIntroInjected())
        {
            MessageBox.Show(@"All patching already done!", @"Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var patchResult =
            MessageBox.Show(
                @"Note! This is permanent and will require a Steam validate to restore the intros. Continue?",
                @"Wait!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (patchResult != DialogResult.Yes) return;
        var (injected, message) = _injector.InjectNoIntros();
        if (injected)
        {
            WriteLog(message);
            var dlgResult = MessageBox.Show(
                @"Intros have been disabled, would you like to apply the patch now?",
                @"Done!", MessageBoxButtons.YesNo);
            if (dlgResult == DialogResult.Yes) BtnPatch_Click(sender, e);
        }
        else
        {
            WriteLog(message, true);
            MessageBox.Show(@"There was an issue patching out intros. Validate Steam files and try again.", @"Hmmm",
                MessageBoxButtons.OK);
        }
    }

    private void BtnRemovePatch_Click(object sender, EventArgs e)
    {
        if (IsGameRunning()) return;
        WriteLog(_injector.Remove());
        CheckPatched();
    }

    private void BtnRestore_Click(object sender, EventArgs e)
    {
        var result =
            MessageBox.Show(
                @"This will restore any backed up Assembly-CSharp.dll. You will need to re-patch to use mods. Continue?",
                @"Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;
        try
        {
            File.Copy(Path.Combine(_gameLocation.location, "Graveyard Keeper_Data\\Managed\\dep\\Assembly-CSharp.dll"),
                Path.Combine(_gameLocation.location, "Graveyard Keeper_Data\\Managed\\Assembly-CSharp.dll"), true);
            FrmMain_Load(sender, e);
            WriteLog("Restored Assembly-CSharp.dll from the Graveyard Keeper_Data\\Managed\\dep directory.");
        }
        catch (FileNotFoundException)
        {
            WriteLog("A backed up Assembly-CSharp.dll could not be found.", true);
        }
        catch (Exception ex)
        {
            WriteLog($"An error occurred: {ex.Message}.", true);
        }
    }

    private void BtnRunGame_Click(object sender, EventArgs e)
    {
        RunGame();
    }

    private void CheckAllModsActive()
    {
        ChkToggleMods.Checked = true;
        foreach (var unused in _modList.Where(mod => mod.Enable == false))
        {
            ChkToggleMods.Checked = false;
            break;
        }
    }

    private void ChecklistToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        if (!_gameLocation.found) return;
        _frmChecklist ??= new FrmChecklist(_injector, _gameLocation.location, _modLocation);
        _frmChecklist.ShowDialog();
        _frmChecklist = null;
    }

    private void CheckPatched()
    {
        if (!_gameLocation.found) return;

        _injector = new Injector(_path);
        if (_injector.IsInjected())
        {
            LblPatched.Text = @"Mod Injector Installed";
            LblPatched.ForeColor = Color.Green;
            BtnPatch.Enabled = false;
            BtnRemovePatch.Enabled = true;
        }
        else
        {
            LblPatched.Text = @"Mod Injector Not Installed";
            LblPatched.ForeColor = Color.Red;
            BtnPatch.Enabled = true;
            BtnRemovePatch.Enabled = false;
        }

        if (_injector.IsNoIntroInjected())
        {
            if (ModInList("intros"))
            {
                LblIntroPatched.Text = @"Intros Removed (via mod and patch?).";
                LblIntroPatched.ForeColor = Color.DarkOrange;
            }
            else
            {
                LblIntroPatched.Text = @"Intros Removed (via patch).";
                LblIntroPatched.ForeColor = Color.Green;
            }
        }
        else
        {
            if (ModInList("intros"))
            {
                LblIntroPatched.Text = @"Intros Removed (via mod).";
                LblIntroPatched.ForeColor = Color.Green;
            }
            else
            {
                LblIntroPatched.Text = @"Intros Not Removed";
                LblIntroPatched.ForeColor = Color.Red;
            }
        }

        try
        {
            if (!CleanMd5Hashes.Contains(Utilities.CalculateMd5(Path.Combine(_gameLocation.location,
                    "Graveyard Keeper_Data\\Managed\\Assembly-CSharp.dll")))) return;

            File.Copy(Path.Combine(_gameLocation.location, "Graveyard Keeper_Data\\Managed\\Assembly-CSharp.dll"),
                Path.Combine(_gameLocation.location, "Graveyard Keeper_Data\\Managed\\dep\\Assembly-CSharp.dll"),
                true);
            WriteLog("Clean Assembly-CSharp.dll detected. Backing up to Graveyard Keeper_Data\\Managed\\dep");
        }
        catch (FileNotFoundException)
        {
            WriteLog("Assembly-CSharp.dll not found. Probably need to check that out.", true);
        }
        catch (Exception)
        {
            //
        }
    }

    private void CheckQueueEverything()
    {
        var foundQueueEverything = FindMod("QueueEverything");
        var foundExhaustless = FindMod("Exhaust-less");
        var foundFasterCraft = FindMod("FasterCraft");
        var showOrderMessage = false;
        if (foundQueueEverything != null)
        {
            if (foundExhaustless != null)
            {
                if (foundQueueEverything.LoadOrder < foundExhaustless.LoadOrder)
                {
                    showOrderMessage = true;
                }
            }

            if (foundFasterCraft != null)
            {
                if (foundQueueEverything.LoadOrder < foundFasterCraft.LoadOrder)
                {
                    showOrderMessage = true;
                }
            }
        }

        if (showOrderMessage)
        {
            MessageBox.Show(
                @"It seems you have Queue Everything!* and Exhaust-less/FasterCraft set to an invalid load order. Please ensure that Queue Everything is further down the load order than both of those mods, or it won't detect them.",
                @"Load Order Issue", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }

    private void ChkLaunchExeDirectly_CheckStateChanged(object sender, EventArgs e)
    {
        Properties.Settings.Default.LaunchDirectly = ChkLaunchExeDirectly.Checked;
        Properties.Settings.Default.Save();
    }

    private void ChkToggleMods_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in DgvMods.Rows)
        {
            if (ChkToggleMods.Checked)
            {
                row.Cells[0].Value = 1;
                ToggleModEnabled(true, row.Index);
            }
            else
            {
                row.Cells[0].Value = 0;
                ToggleModEnabled(false, row.Index);
            }
        }
    }

    private void DgvMods_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == 0)
        {
            if (e.RowIndex <= 0) return;
            if (DgvMods[e.ColumnIndex, e.RowIndex].Value.Equals(0))
            {
                DgvMods[e.ColumnIndex, e.RowIndex].Value = 1;
                ToggleModEnabled(true, e.RowIndex);
            }
            else
            {
                DgvMods[e.ColumnIndex, e.RowIndex].Value = 0;
                ToggleModEnabled(false, e.RowIndex);
            }
        }

        if (e.ColumnIndex == 6)
        {
            var foundMod = _modList.FirstOrDefault(x => x.Id == DgvMods[7, e.RowIndex].Value.ToString());

            if (foundMod == null || foundMod.Config == string.Empty) return;
            try
            {
                WriteLog($"Opening {foundMod.Config} for {foundMod.DisplayName}");

                _frmConfigEdit ??= new FrmConfigEdit(ref foundMod, ref DgvLog, _gameLocation.location);
                _frmConfigEdit.ShowDialog();
                _frmConfigEdit = null;
            }
            catch (Exception ex)
            {
                WriteLog($"Issue opening {foundMod.Config} for {foundMod.DisplayName}. Exception: {ex.Message}");
            }
        }
    }

    private void DgvMods_DragDrop(object sender, DragEventArgs e)
    {
        var clientPoint = DgvMods.PointToClient(new Point(e.X, e.Y));
        _rowIndexOfItemUnderMouseToDrop =
            DgvMods.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
        if (e.Effect != DragDropEffects.Move) return;
        if (_rowIndexOfItemUnderMouseToDrop < 0)
        {
            return;
        }

        DgvMods.Rows.RemoveAt(_rowIndexFromMouseDown);
        if (e.Data.GetData(
                typeof(DataGridViewRow)) is DataGridViewRow rowToMove)
            DgvMods.Rows.Insert(_rowIndexOfItemUnderMouseToDrop, rowToMove);
        UpdateLoadOrders();
    }

    private void DgvMods_DragOver(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;
    }

    private void DgvMods_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (DgvMods.HitTest(e.X, e.Y).ColumnIndex is 0 or 6) return;
        if (DgvMods.SelectedRows.Count > 1)
        {
            return;
        }
        if (!_gameLocation.found) return;
        QMod modFound = null;
        try
        {
            modFound = FindMod(DgvMods.CurrentRow?.Cells[7].Value.ToString());
            if (modFound != null)
                Process.Start(modFound.ModAssemblyPath);
        }
        catch (Exception)
        {
            WriteLog($"Issue locating folder for {modFound?.DisplayName}.", true);
        }
    }

    private void DgvMods_MouseDown(object sender, MouseEventArgs e)
    {
        _rowIndexFromMouseDown = DgvMods.HitTest(e.X, e.Y).RowIndex;
        if (_rowIndexFromMouseDown != -1)
        {
            var dragSize = SystemInformation.DragSize;
            _dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                    e.Y - (dragSize.Height / 2)),
                dragSize);
        }
        else
            _dragBoxFromMouseDown = Rectangle.Empty;
    }

    private void DgvMods_MouseMove(object sender, MouseEventArgs e)
    {
        if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
        if (_dragBoxFromMouseDown != Rectangle.Empty &&
            !_dragBoxFromMouseDown.Contains(e.X, e.Y))
        {
            DgvMods.DoDragDrop(
                DgvMods.Rows[_rowIndexFromMouseDown],
                DragDropEffects.Move);
        }
    }

    private void ExitToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void ExitToolStripMenuItem2_Click(object sender, EventArgs e)
    {
        ExitToolStripMenuItem1_Click(sender, e);
    }

    private QMod FindMod(string modId)
    {
        return _modList.FirstOrDefault(x => x.Id == modId);
    }

    private void FrmMain_Load(object sender, EventArgs e)
    {
        SetLocations();
        LoadMods();
        UpdateModJsons();
        LoadMods();
        BtnRefresh.Enabled = _modList.Count > 0;
        DgvMods.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        DgvMods.Sort(DgvMods.Columns[1], ListSortDirection.Ascending);
        DgvMods.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        DgvMods.AllowUserToResizeRows = false;
        DgvMods.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        DgvMods.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        DgvMods.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        DgvMods.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        Text = $@"QMod Manager Reloaded v{Assembly.GetExecutingAssembly().GetName().Version}";
    }

    private void FrmMain_Resize(object sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            ShowInTaskbar = false;
        }
    }

    private void LaunchGameToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BtnRunGame_Click(sender, e);
    }

    private void LoadMods()
    {
        try
        {
            LblErrors.Text = string.Empty;
        _modList.Clear();
        DgvMods.Rows.Clear();
        if (!_gameLocation.found) return;

        var dllFiles =
            Directory.EnumerateDirectories(_modLocation).SelectMany(
                directory => Directory.EnumerateFiles(directory, "*.dll"));

        foreach (var dllFile in dllFiles)
        {
            // GetModEntryPoint(dllFile);
            var path = new FileInfo(dllFile).DirectoryName;
            if (path == null) continue;
            var dllFileName = new FileInfo(dllFile).Name;
            var modJsonFile = Directory.GetFiles(path, "mod.json", SearchOption.TopDirectoryOnly);
            var infoJsonFile = Directory.GetFiles(path, "info.json", SearchOption.TopDirectoryOnly);
            string jsonFile = null;
            if (modJsonFile.Length == 1 && infoJsonFile.Length == 1)
            {
                WriteLog(
                    $"Multiple JSON detected for {dllFileName}. Please remove one. Either mod.json or info.json, not both.");
                continue;
            }

            if (modJsonFile.Length == 1)
            {
                jsonFile = modJsonFile[0];
            }
            else if (infoJsonFile.Length == 1)
            {
                File.Copy(infoJsonFile[0], Path.Combine(new FileInfo(infoJsonFile[0]).DirectoryName!, "mod.json"),
                    true);
                File.Delete(infoJsonFile[0]);
                jsonFile = "mod.json";
            }
            else
            {
                var createResult = CreateJson(dllFile);
                if (createResult == false)
                {
                    WriteLog("Error creating JSON file.", true);
                }
                else
                {
                    jsonFile = "mod.json";
                }
            }

            if (jsonFile == null)
            {
                WriteLog($"{dllFileName} didn't have a valid json.", true);
            }
            else
            {
                var mod = QMod.FromJsonFile(Path.Combine(path, jsonFile));
                if (mod == null)
                {
                    WriteLog($"{dllFileName} didn't have a valid json.", true);
                }
                else
                {
                    mod.ModAssemblyPath = path;

                    if (!string.IsNullOrEmpty(mod.EntryMethod))
                    {
                        if (mod.LoadOrder <= 0)
                        {
                            mod.LoadOrder = _modList.Count + 1;
                            var json = JsonSerializer.Serialize(mod, JsonOptions);
                            File.WriteAllText(Path.Combine(mod.ModAssemblyPath, "mod.json"), json);
                        }

                        var isModCompatible = IsModCompatible(Path.Combine(mod.ModAssemblyPath, dllFileName));
                        var (configExists, configFile) = GetModConfigIfItExists(mod.ModAssemblyPath);
                        int rowIndex;
                        mod.Config = "none";
                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        if (configExists)
                        {
                            rowIndex = DgvMods.Rows.Add(Convert.ToInt32(mod.Enable), mod.LoadOrder, mod.DisplayName, mod.Description,
                                mod.Version, mod.Author, "...", mod.Id);
                            mod.Config = Path.GetFileName(configFile);
                        }
                        else
                        {
                            rowIndex = DgvMods.Rows.Add(Convert.ToInt32(mod.Enable), mod.LoadOrder, mod.DisplayName, mod.Description,
                                mod.Version, mod.Author, string.Empty, mod.Id);
                        }

                        var row = DgvMods.Rows[rowIndex];
                        if (!isModCompatible)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightCoral;
                            WriteLog(
                                mod.DisplayName +
                                " added, but it's not Harmony 2 compatible, and will not load without updating by the author.",
                                true);
                        }
                        else
                        {
                            WriteLog(mod.DisplayName + " added.");
                        }
                        _modList.Add(mod);
                    }
                    else
                    {
                        WriteLog(mod.DisplayName + " had issues and wasn't loaded.", true);
                    }
                }
            }
        }

        DgvMods.Sort(DgvMods.Columns[1], ListSortDirection.Ascending);
        WriteLog(
            "All mods with an entry point added. This doesn't mean they'll load correctly or function if they do load.");
        UpdateModJsons();
        }
        catch (Exception ex)
        {
            WriteLog($"LoadMods() ERROR: {ex.Message}", true);
        }

        BtnRefresh.Enabled = _modList.Count > 0;

        CheckQueueEverything();
        CheckAllModsActive();
        CheckPatched();
    }

    private void ModifyResolutionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!_gameLocation.found) return;
        _frmResModifier ??= new FrmResModifier(ref DgvLog, _gameLocation.location);
        _frmResModifier.ShowDialog();
        _frmResModifier = null;
    }

    private bool ModInList(string mod)
    {
        return _modList.Any(x => x.DisplayName.ToLower().Contains(mod.ToLower()));
    }

    private void OpenGameDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BtnOpenGameDir_Click(sender, e);
    }

    private void OpenmModDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BtnOpenModDir_Click(sender, e);
    }

    private void OpenSaveDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData")!, @"..\", "LocalLow\\Lazy Bear Games\\Graveyard Keeper"));
        }
        catch (Exception ex)
        {
            WriteLog($"{ex.Message}", true);
        }
    }

    private void OpenUnityLogToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData")!, @"..\", "LocalLow\\Lazy Bear Games\\Graveyard Keeper\\Player.log"));
        }
        catch (Exception ex)
        {
            WriteLog($"{ex.Message}", true);
        }
    }

    private void RestoreWindowToolStripMenuItem_Click(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Normal;
        Focus();
        ShowInTaskbar = true;
        DgvLog.FirstDisplayedScrollingRowIndex = DgvLog.RowCount - 1;
    }

    private void RunGame()
    {
        try
        {
            if (Properties.Settings.Default.LaunchDirectly)
            {
                RunDirect();
                return;
            }

            if (IsSteamCopy())
            {
                Console.WriteLine(@"Steam Copy: TRUE");
                using var steam = new Process();
                steam.StartInfo.FileName = "steam://rungameid/599140";
                steam.Start();
            }
            else
            {
                Console.WriteLine(@"Steam Copy: FALSE");
                RunDirect();
            }

            void RunDirect()
            {
                Console.WriteLine(@"Running Direct: TRUE");
                var path = Path.Combine(_gameLocation.location, "Graveyard Keeper.exe");
                using var gyk = new Process();
                gyk.StartInfo.FileName = path;
                gyk.StartInfo.UseShellExecute = false;
                gyk.StartInfo.WorkingDirectory = _gameLocation.location;
                gyk.Start();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(@"Error launching game: " + ex.Message, @"Error", MessageBoxButtons.OK);
        }
        finally
        {
            WindowState = FormWindowState.Minimized;
        }
    }

    private void SetLocations()
    {
        _path = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\"));
        //var di = new DirectoryInfo(Path.Combine(Application.StartupPath,@"..\..", "Graveyard Keeper_Data\\Managed\\Graveyard Keeper.exe"));
        var fi = new FileInfo(Path.Combine(_path, "Graveyard Keeper.exe"));
        Console.WriteLine(@$"Path: {_path}");

        if (fi.Exists)
        {
            LblPatched.Visible = true;
            LblIntroPatched.Visible = true;
            _gameLocation.found = true;
            _gameLocation.location = _path;
            TxtGameLocation.Text = _gameLocation.location;
            _modLocation = Path.Combine(_gameLocation.location, "QMods");
            TxtModFolderLocation.Text = _modLocation;
            Properties.Settings.Default.GamePath = _gameLocation.location;
            Properties.Settings.Default.Save();
        }
        else
        {
            LblPatched.Visible = false;
            LblIntroPatched.Visible = false;
            _gameLocation.found = false;
            TxtGameLocation.Text = @"Looks like I'm not installed in the correct directory.";
            MessageBox.Show(@"Please ensure I've been installed directly into the Managed directory within the Graveyard Keeper directory.", @"Wrong directory.",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        if (!_gameLocation.found) return;
        if (new DirectoryInfo(_modLocation).Exists) return;
        Directory.CreateDirectory(_modLocation);
        WriteLog("INFO: QMods directory created.");
    }

    private void ToggleModEnabled(bool enabled, int row)
    {
        try
        {
            var modFound = FindMod(DgvMods.Rows[row].Cells[7].Value.ToString());

            if (modFound == null) return;
            modFound.Enable = enabled;

            var newJson = JsonSerializer.Serialize(modFound, JsonOptions);
            File.WriteAllText(Path.Combine(modFound.ModAssemblyPath, "mod.json"), newJson);
        }
        catch (Exception)
        {
            WriteLog("Issues toggling mod functionality.", true);
        }
    }

    private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
            WindowState = FormWindowState.Normal;
        else
        {
            TopMost = true;
            Focus();
            BringToFront();
            TopMost = false;
        }
        Focus();
        ShowInTaskbar = true;
        DgvLog.FirstDisplayedScrollingRowIndex = DgvLog.RowCount - 1;
    }

    private void UpdateLoadOrders()
    {
        foreach (DataGridViewRow row in DgvMods.Rows)
        {
            foreach (var mod in _modList.Where(mod =>
                         mod.Id == DgvMods.Rows[row.Index].Cells[7].Value.ToString()))
            {
                DgvMods.Rows[row.Index].Cells[0].Value = row.Index + 1;
                mod.LoadOrder = row.Index + 1;
                var json = JsonSerializer.Serialize(mod, JsonOptions);
                File.WriteAllText(Path.Combine(mod.ModAssemblyPath, "mod.json"), json);
            }
        }

        CheckQueueEverything();
    }

    private void UpdateModJsons()
    {
        WriteLog("Updating mod.json files for all installed mods. Information is pulled from the mods directly.", false);
        foreach (var mod in _modList)
        {
            var (exists, file) = GetModConfigIfItExists(mod.ModAssemblyPath);
            mod.Config = "none";
            if (exists)
            {
                mod.Config = Path.GetFileName(file);
            }
            var path = Path.Combine(mod.ModAssemblyPath, mod.AssemblyName);
            var (namesp, type, method, _) = GetModEntryPoint(path);
            var modInfo = FileVersionInfo.GetVersionInfo(path);
            mod.Author = modInfo.CompanyName;
            mod.DisplayName = modInfo.ProductName;
            mod.Description = modInfo.FileDescription;
            mod.Version = modInfo.ProductVersion;
            mod.EntryMethod = $"{namesp}.{type}.{method}";
            var newJson = JsonSerializer.Serialize(mod, JsonOptions);
            File.WriteAllText(Path.Combine(mod.ModAssemblyPath, "mod.json"), newJson);
        }
    }

    private void WriteLog(string message, bool error = false)
    {
        var dt = DateTime.Now;
        var rowIndex = DgvLog.Rows.Add(dt.ToLongTimeString(), message);
        var row = DgvLog.Rows[rowIndex];
        if (error)
        {
            row.DefaultCellStyle.BackColor = Color.LightCoral;
        }

        string logMessage;
        if (error)
        {
            logMessage = "-----------------------------------------\n";
            logMessage += dt.ToShortDateString() + " " + dt.ToLongTimeString() + " : [ERROR] : " + message + "\n";
            logMessage += "-----------------------------------------";
        }
        else
        {
            logMessage = dt.ToShortDateString() + " " + dt.ToLongTimeString() + " : " + message;
        }
        Utilities.WriteLog(logMessage, _gameLocation.location);

        var errors = DgvLog.Rows.Cast<DataGridViewRow>().Count(r => r.DefaultCellStyle.BackColor == Color.LightCoral);
        if (errors > 0)
        {
            LblErrors.Visible = true;
            LblErrors.Text = $@"Errors: {errors}";
        }
        else
        {
            LblErrors.Text = "";
            LblErrors.Visible = false;
        }

        DgvLog.FirstDisplayedScrollingRowIndex = DgvLog.RowCount - 1;
    }

    private void DgvMods_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
        if(e.RowIndex<=0) return;
        var cell = (DataGridViewLinkCell)DgvMods[6, e.RowIndex];
        cell.LinkColor = Color.White;
        cell.VisitedLinkColor = Color.White;
        cell.ActiveLinkColor = Color.White;
    }

    private void DgvMods_RowLeave(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex <= 0) return;
        var cell = (DataGridViewLinkCell)DgvMods[6, e.RowIndex];
        cell.LinkColor = Color.DarkBlue;
        cell.VisitedLinkColor = Color.DarkBlue;
        cell.ActiveLinkColor = Color.DarkBlue;
    }
}