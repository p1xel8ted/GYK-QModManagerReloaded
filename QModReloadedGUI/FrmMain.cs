using Ionic.Zip;
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
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using static QModReloadedGUI.Utilities;
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
    private bool _canCheckForUpdates = true;
    private QMod _contextMenuMod;
    private int _modUpdateCounter;
    private static readonly string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QMR");
    private readonly string _dataPath = Path.Combine(BasePath, "k.json");
    private Rectangle _dragBoxFromMouseDown;
    private FrmAbout _frmAbout;
    private FrmChecklist _frmChecklist;
    private FrmConfigEdit _frmConfigEdit;
    private FrmNexus _frmNexus;
    private FrmOptions _frmOptions;
    private FrmResModifier _frmResModifier;
    private (string location, bool found) _gameLocation;
    private Injector _injector;
    private static string _modLocation = string.Empty;
    private int _rowIndexFromMouseDown;
    private int _rowIndexOfItemUnderMouseToDrop;
    private Settings _settings;
    private Timer _updateTimer;

    public FrmMain(Settings settings)
    {
        _settings = settings;
        InitializeComponent();
    }

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
            NexusId = -1,
            EntryMethod = found ? $"{namesp}.{type}.{method}" : $"{fileNameWithoutExt}.MainPatcher.Patch",
            Version = modInfo.FileVersion
        };
        var newJson = JsonSerializer.Serialize(newMod, JsonOptions);
        if (path == null) return false;
        File.WriteAllText(Path.Combine(path, "mod.json"), newJson);
        var files = new FileInfo(Path.Combine(path, "mod.json"));
        return files.Exists;
    }

    private static string FixVersion(string version)
    {
        var dotCount = version.Count(c => c == '.');
        var fixedVersion = dotCount switch
        {
            0 =>
                //i.e 1
                $"{version}.0.0.0",
            1 =>
                //ie 1.0
                $"{version}.0.0",
            2 =>
                //ie 1.2.3
                $"{version}.0",
            _ => "0.0.0.0"
        };

        return fixedVersion;
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

    private void AddMod(string file, bool update = false, QMod mod = null)
    {
        try
        {
            var modZip = new FileInfo(file);
            if (!modZip.Exists) return;
            using (var zip = Ionic.Zip.ZipFile.Read(modZip.FullName))
            {
                foreach (var e in zip)
                {
                    if (e.ToString().EndsWith("dll"))
                    {
                        var newFile = e.ToString().Remove(0, 10);
                        var path = _modLocation + "\\" + newFile.Substring(0, newFile.Length - 4);
   
                        zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                        break;
                    }
                    zip.ExtractAll(_modLocation, ExtractExistingFileAction.OverwriteSilently);
                    break;
                }
            }

            if (update)
            {
                if (mod != null)
                {
                    mod.UpdateAvailable = false;
                    WriteLog($"[ZIP]: Updated {mod.DisplayName}");
                }
            }
            else
            {
                WriteLog($"[ZIP]: Extracted {modZip.Name}");
            }

            BtnRefresh_Click(null,null);
        }
        catch (Exception ex)
        {
            WriteLog($"[ZIP]: {ex.Message}", true);
        }

        if (!update) return;
        _modUpdateCounter--;
        if (_modUpdateCounter > 0) return;
        UpdateProgress.Value = 0;
        UpdateProgress.Visible = false;
    }

    private void BtnAddMod_Click(object sender, EventArgs e)
    {
        var dlgResult = DlgFile.ShowDialog(this);
        if (dlgResult == DialogResult.OK)
            foreach (var zip in DlgFile.FileNames)
            {
                AddMod(zip);
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
            MessageBox.Show(@"No mods installed to refresh!", @"Umm", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        LblErrors.Visible = false;
        UpdateModJson(true);
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

    private void CheckForUpdates()
    {
        _modUpdateCounter = 0;
        UpdateProgress.Visible = true;
        _updateTimer = new Timer
        {
            Interval = 500,
            Enabled = true
        };
        _updateTimer.Tick += UpdateTimerTick;
        _updateTimer.Start();

        void UpdateTimerTick(object sender, EventArgs e)
        {
            if (UpdateProgress.Value >= UpdateProgress.Maximum)
            {
                _canCheckForUpdates = true;
                UpdateProgress.Visible = false;
                UpdateModJson(true);
                _updateTimer.Stop();
            }
            else
            {
                _canCheckForUpdates = false;
            }

            updatesToolStripMenuItem.Enabled = _canCheckForUpdates;
        }

        if (!_canCheckForUpdates) return;
        if (_settings.ApiKey == null)
        {
            MessageBox.Show(@"You haven't set your API key. This is required for NexusMods to allow access to the API.",
                @"No API Key", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        PairedKeys pairedKeys = null;
        if (File.Exists(_dataPath))
        {
            pairedKeys = JsonSerializer.Deserialize<PairedKeys>(File.ReadAllText(_dataPath),
               new JsonSerializerOptions { AllowTrailingCommas = true });
        }
        if (pairedKeys?.Vector is null)
        {
            return;
        }

        foreach (DataGridViewRow row in DgvMods.Rows)
        {
            row.DefaultCellStyle.BackColor = Color.White;
        }

        UpdateProgress.Visible = true;
        UpdateProgress.Value = 0;
        UpdateProgress.Maximum = _modList.Count;

        foreach (var mod in _modList)
        {
            var modUpdate = new WebClient();

            modUpdate.Headers.Add("apikey", Obscure.Decrypt(_settings.ApiKey, pairedKeys.Lock, pairedKeys.Vector));

            modUpdate.DownloadStringCompleted += CheckForUpdatesDownloadedCompleted;
            modUpdate.Headers.Add("Application-Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            modUpdate.Headers.Add("Application-Name", "QMod-Manager-Reloaded");
            modUpdate.Headers.Add("User-Agent", $"QMod-Manager-Reloaded/{Assembly.GetExecutingAssembly().GetName().Version} {Environment.OSVersion}");
            if (mod.DisplayName.ToLower().Contains("harmony"))
            {
                WriteLog($"Skipping Harmony1to2 Converted Mod {mod.DisplayName}");
                UpdateProgress.Value++;
                continue;
            }
            modUpdate.DownloadStringAsync(new Uri($"https://api.nexusmods.com/v1/games/graveyardkeeper/mods/{mod.NexusId}.json"));

            void CheckForUpdatesDownloadedCompleted(object sender, DownloadStringCompletedEventArgs args)
            {
                LblNexusRequests.Text = UpdateRequestCounts(modUpdate.ResponseHeaders, _settings.UserName, _settings.IsPremium);
                try
                {
                    ProcessJson(mod, args.Result);
                }
                catch (Exception ex) when (ex.InnerException != null)
                {
                    WriteLog($"[Updates]: Mod: {mod.DisplayName}, {ex.InnerException.Message}", true);
                }
                catch (Exception ex)
                {
                    WriteLog($"[Updates]: Mod: {mod.DisplayName}, {ex.Message}", true);
                }
                UpdateProgress.Value++;
            }
        }

        var qmrUpdate = new WebClient();

        qmrUpdate.Headers.Add("apikey", Obscure.Decrypt(_settings.ApiKey, pairedKeys.Lock, pairedKeys.Vector));

        qmrUpdate.Headers.Add("Application-Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        qmrUpdate.Headers.Add("Application-Name", "QMod-Manager-Reloaded");
        qmrUpdate.Headers.Add("User-Agent", $"QMod-Manager-Reloaded/{Assembly.GetExecutingAssembly().GetName().Version} {Environment.OSVersion}");
        qmrUpdate.DownloadStringCompleted += CheckForQmrUpdatesCompleted;
        qmrUpdate.DownloadStringAsync(new Uri("https://api.nexusmods.com/v1/games/graveyardkeeper/mods/40.json"));
        void CheckForQmrUpdatesCompleted(object sender, DownloadStringCompletedEventArgs args)
        {
            LblNexusRequests.Text = UpdateRequestCounts(qmrUpdate.ResponseHeaders, _settings.UserName, _settings.IsPremium);
            try
            {
                var nexusMod = JsonSerializer.Deserialize<NexusMod>(args.Result);
                if (nexusMod == null) return;
                var currentVersion = Version.Parse(FixVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString()));
                var newVersion = Version.Parse(FixVersion(nexusMod.Version));
                var result = currentVersion.CompareTo(newVersion);
                if (result < 0)
                {
                    Text = $@"QMod Manager Reloaded v{Assembly.GetExecutingAssembly().GetName().Version} - Update is available on NexusMods.";
                    WriteLog("Update available for QMod Manager Reloaded on NexusMods!", error: true);
                }
                else
                {
                    Text = $@"QMod Manager Reloaded v{Assembly.GetExecutingAssembly().GetName().Version}";
                }
            }
            catch (Exception ex) when (ex.InnerException != null)
            {
                WriteLog($"[Updates]: QMR, {ex.InnerException.Message}", true);
            }
            catch (Exception ex)
            {
                WriteLog($"[Updates]: QMR, {ex.Message}", true);
            }
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
            if (!CleanMd5Hashes.Contains(CalculateMd5(Path.Combine(_gameLocation.location,
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
        const string modIdQueueEverything = "QueueEverything";
        const string modIdExhaustless = "Exhaust-less";
        const string modIdFasterCraft = "FasterCraft";
        const string modIdINeedSticks = "INeedSticks";

        var rowIdQueueEverything = FindModRow(modIdQueueEverything);
        var rowIdExhaustless = FindModRow(modIdExhaustless);
        var rowIdFasterCraft = FindModRow(modIdFasterCraft);
        var rowIdINeedSticks = FindModRow(modIdINeedSticks);

        var foundQueueEverything = FindMod(modIdQueueEverything);
        var foundExhaustless = FindMod(modIdExhaustless);
        var foundFasterCraft = FindMod(modIdFasterCraft);
        var foundINeedSticks = FindMod(modIdINeedSticks);

        var showOrderMessage = false;
        if (foundQueueEverything != null)
        {
            if (foundExhaustless != null)
            {
                if (foundQueueEverything.LoadOrder < foundExhaustless.LoadOrder)
                {
                    DgvMods.Rows[rowIdQueueEverything].DefaultCellStyle.BackColor = Color.LightCoral;
                    DgvMods.Rows[rowIdExhaustless].DefaultCellStyle.BackColor = Color.LightCoral;
                    showOrderMessage = true;
                }
                else
                {
                    DgvMods.Rows[rowIdQueueEverything].DefaultCellStyle.BackColor = Color.White;
                    DgvMods.Rows[rowIdExhaustless].DefaultCellStyle.BackColor = Color.White;
                }
            }

            if (foundFasterCraft != null)
            {
                if (foundQueueEverything.LoadOrder < foundFasterCraft.LoadOrder)
                {
                    DgvMods.Rows[rowIdQueueEverything].DefaultCellStyle.BackColor = Color.LightCoral;
                    DgvMods.Rows[rowIdFasterCraft].DefaultCellStyle.BackColor = Color.LightCoral;
                    showOrderMessage = true;
                }
                else
                {
                    DgvMods.Rows[rowIdQueueEverything].DefaultCellStyle.BackColor = Color.White;
                    DgvMods.Rows[rowIdFasterCraft].DefaultCellStyle.BackColor = Color.White;
                }
            }

            if (foundINeedSticks != null)
            {
                if (foundQueueEverything.LoadOrder > foundINeedSticks.LoadOrder)
                {
                    DgvMods.Rows[rowIdQueueEverything].DefaultCellStyle.BackColor = Color.LightCoral;
                    DgvMods.Rows[rowIdINeedSticks].DefaultCellStyle.BackColor = Color.LightCoral;
                    showOrderMessage = true;
                }
                else
                {
                    DgvMods.Rows[rowIdQueueEverything].DefaultCellStyle.BackColor = Color.White;
                    DgvMods.Rows[rowIdINeedSticks].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        if (showOrderMessage)
        {
            MessageBox.Show(
                @"It seems you have all or some of the following mods installed. Please ensure their load order is as follows:" +
                @"\n\nExhaust-less/FasterCraft - doesn't matter which order." +
                @"\nQueue Everything!* - must come after the above two." +
                @"\nI Neeeed Sticks! - must come after the above three." +
                @"\n\nIt doesn't matter if other mods are in-between.",
                @"Load Order Issue", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
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
            if (e.RowIndex < 0) return;
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

    private void DgvMods_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;
        try
        {
            DgvMods.CurrentCell = DgvMods.Rows[e.RowIndex].Cells[e.ColumnIndex];
            DgvMods.Rows[e.RowIndex].Selected = true;
            DgvMods.Focus();
        }
        catch (Exception)
        {
            // ignored
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
        if (e.Button != MouseButtons.Left) return;
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

    private void DgvMods_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex <= 0) return;
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

    private int FindModRow(string modId)
    {
        foreach (DataGridViewRow row in DgvMods.Rows)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.ColumnIndex != 7) continue;
                if (cell.Value.Equals(modId))
                {
                    return cell.RowIndex;
                }
            }
        }
        return -1;
    }

    private void FrmMain_Load(object sender, EventArgs e)
    {
        Directory.CreateDirectory(BasePath);
        SetLocations();
        LoadMods();
        UpdateModJson();
        LoadMods();

        BtnRefresh.Enabled = _modList.Count > 0;
        DgvMods.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        DgvMods.Sort(DgvMods.Columns[1], ListSortDirection.Ascending);
        DgvMods.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        DgvMods.AllowUserToResizeRows = false;
        DgvMods.AutoGenerateColumns = true;
        DgvMods.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        DgvMods.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        DgvMods.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        DgvMods.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        Text = $@"QMod Manager Reloaded v{Assembly.GetExecutingAssembly().GetName().Version}";

        if (_settings.UpdateOnStartup)
        {
            CheckForUpdates();
        }
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
                            mod.Config = "none";
                            int rowIndex;
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

                            DgvMods.Rows[rowIndex].DefaultCellStyle.BackColor = mod.UpdateAvailable ? Color.LightGreen : Color.White;

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
            UpdateModJson();
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

    private void ModListCtxMenu_Opening(object sender, CancelEventArgs e)
    {
        if (DgvMods.CurrentRow == null) return;
        _contextMenuMod = FindMod(DgvMods.CurrentRow.Cells[7].Value.ToString());
        var (exists, _) = GetModConfigIfItExists(_contextMenuMod.ModAssemblyPath);
        openConfigToolStripMenuItem.Enabled = exists;
        openConfigToolStripMenuItem.Visible = exists;
        ModMenuName.Text = _contextMenuMod.DisplayName;

        ModMenuUpdate.Visible = _settings.IsPremium;
        UpdateDivider.Visible = _settings.IsPremium;

        ModMenuUpdate.Enabled = _contextMenuMod.UpdateAvailable;
        ModMenuName.Enabled = _contextMenuMod.NexusId > 0;
    }

    private void ModMenuName_Click(object sender, EventArgs e)
    {
        try
        {
            modListCtxMenu.Close();
            Process.Start($"https://www.nexusmods.com/graveyardkeeper/mods/{_contextMenuMod.NexusId}");
        }
        catch (Exception)
        {
            WriteLog($"Error launching Nexus page for {_contextMenuMod.DisplayName} ({_contextMenuMod.NexusId})", true);
        }
    }

    private void NexusAPIKeyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _frmNexus ??= new FrmNexus(_dataPath, ref LblNexusRequests, ref _settings);
        _frmNexus.ShowDialog();
        _frmNexus = null;
    }

    private void OpenConfigToolStripMenuItem_Click(object sender, EventArgs e)
    {
   try
        {
            modListCtxMenu.Close();
            WriteLog($"Opening {_contextMenuMod.Config} for {_contextMenuMod.DisplayName}");

            _frmConfigEdit ??= new FrmConfigEdit(ref _contextMenuMod, ref DgvLog, _gameLocation.location);
            _frmConfigEdit.ShowDialog();
            _frmConfigEdit = null;
        }
        catch (Exception ex)
        {
            WriteLog($"Issue opening {_contextMenuMod.Config} for {_contextMenuMod.DisplayName}. Exception: {ex.Message}");
        }
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

    private void ProcessJson(QMod mod, string results)
    {
        try
        {
            var nexusMod = JsonSerializer.Deserialize<NexusMod>(results);
            if (nexusMod == null) return;

            var currentVersion = Version.Parse(FixVersion(mod.Version));
            var newVersion = Version.Parse(FixVersion(nexusMod.Version));
            var result = currentVersion.CompareTo(newVersion);
            if (result < 0)
            {
                _modUpdateCounter++;
                mod.UpdateAvailable = true;
                DgvMods.Rows[FindModRow(mod.Id)].DefaultCellStyle.BackColor = Color.LightGreen;
                WriteLog($"Update available for {mod.DisplayName} on NexusMods!", alert: true);
            }
            else
            {
                mod.UpdateAvailable = false;
                DgvMods.Rows[FindModRow(mod.Id)].DefaultCellStyle.BackColor = Color.White;
            }
        }
        catch (Exception ex) when (ex.InnerException != null)
        {
            WriteLog(
                ex.InnerException.Message.Contains("404")
                    ? $"Updates: Unable to locate Nexus page for {mod.DisplayName}. Is the NexusID in mod.json correct?"
                    : $"Updates: {ex.Message}", true);
        }
        catch (Exception ex)
        {
            WriteLog($"Updates: {ex.Message}", true);
        }
    }

    private void RemoveModToolStripMenuItem_Click(object sender, EventArgs e)
    {
        modListCtxMenu.Close();
        BtnRemove_Click(sender, e);
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
            if (_settings.LaunchDirectly)
            {
                RunDirect();
                return;
            }

            if (IsSteamCopy())
            {
                using var steam = new Process();
                steam.StartInfo.FileName = "steam://rungameid/599140";
                steam.Start();
            }
            else
            {
                RunDirect();
            }

            void RunDirect()
            {
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

        if (fi.Exists)
        {
            LblPatched.Visible = true;
            LblIntroPatched.Visible = true;
            _gameLocation.found = true;
            _gameLocation.location = _path;
            TxtGameLocation.Text = _gameLocation.location;
            _modLocation = Path.Combine(_gameLocation.location, "QMods");
            TxtModFolderLocation.Text = _modLocation;
            _settings.GamePath = _gameLocation.location;
            _settings.Save();
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

    private void TxtFilter_TextChanged(object sender, EventArgs e)
    {
        if (TxtFilter.Text.Length <= 0)
        {
            foreach (DataGridViewRow row in DgvMods.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.White;
            }
            return;
        }

        foreach (DataGridViewRow row in DgvMods.Rows)
        {
            row.DefaultCellStyle.BackColor = Color.White;
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.Value.ToString().ToLower().Contains(TxtFilter.Text.ToLower()))
                {
                    row.DefaultCellStyle.BackColor = Color.Chartreuse;
                }
            }
        }
    }

    private void UpdateLoadOrders()
    {
        foreach (DataGridViewRow row in DgvMods.Rows)
        {
            foreach (var mod in _modList.Where(mod =>
                         mod.Id == DgvMods.Rows[row.Index].Cells[7].Value.ToString()))
            {
                DgvMods.Rows[row.Index].Cells[1].Value = row.Index + 1;
                mod.LoadOrder = row.Index + 1;
                var json = JsonSerializer.Serialize(mod, JsonOptions);
                File.WriteAllText(Path.Combine(mod.ModAssemblyPath, "mod.json"), json);
            }
        }

        CheckQueueEverything();
    }

    private void UpdateModJson(bool silent = false)
    {
        if (!silent)
        {
            WriteLog("Updating mod.json files for all installed mods. Information is pulled from the mods directly.");
        }

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

    private void UpdatesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CheckForUpdates();
    }

    private void WriteLog(string message, bool error = false, bool alert = false)
    {
        var dt = DateTime.Now;
        var rowIndex = DgvLog.Rows.Add(dt.ToLongTimeString(), message);
        var row = DgvLog.Rows[rowIndex];
        if (error)
        {
            row.DefaultCellStyle.BackColor = Color.LightCoral;
        }
        if (alert)
        {
            row.DefaultCellStyle.BackColor = Color.LightGreen;
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

    private void NexusPageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start("https://www.nexusmods.com/graveyardkeeper/mods/40");
        }
        catch (Exception)
        {
            WriteLog($"Error launching Nexus page for QMod Manager Reloaded (NexusID: 40)", true);
        }
    }

    private void ProcessDownloadLink(QMod mod, string url, string apiKey, string fileName)
    {
        var downloadPath = Path.Combine(BasePath, "ModUpdates");
        Directory.CreateDirectory(downloadPath);
        var newFile = Path.Combine(downloadPath, fileName);

        var modDownloader = new WebClient();
        modDownloader.Headers.Add("apikey", apiKey);
        modDownloader.DownloadFileCompleted += DownloadCompleted;
        modDownloader.Headers.Add("Application-Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        modDownloader.Headers.Add("Application-Name", "QMod-Manager-Reloaded");
        modDownloader.Headers.Add("User-Agent", $"QMod-Manager-Reloaded/{Assembly.GetExecutingAssembly().GetName().Version} {Environment.OSVersion}");

        modDownloader.DownloadFileAsync(new Uri(url), newFile);

        void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            AddMod(newFile, true, mod);
        }
        modDownloader.Dispose();
    }

    private void StartUpdates(QMod mod)
    {
        if (!_canCheckForUpdates)
        {
            WriteLog("Please wait for the current update operation to finish.", true);
            return;
        }

        if (mod.NexusId <= 0)
        {
            WriteLog($"[Update]: {mod.DisplayName} doesn't have a valid NexusID. This can be obtained from the Nexus URL for the specified mod.");
            return;
        }
        var modUpdate = new WebClient();
        var pairedKeys = JsonSerializer.Deserialize<PairedKeys>(File.ReadAllText(_dataPath), new JsonSerializerOptions { AllowTrailingCommas = true });
        if (pairedKeys?.Vector is null)
        {
            return;
        }
        modUpdate.Headers.Add("apikey", Obscure.Decrypt(_settings.ApiKey, pairedKeys.Lock, pairedKeys.Vector));

        modUpdate.DownloadStringCompleted += UpdateFilesDownloadedCompleted;
        modUpdate.Headers.Add("Application-Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        modUpdate.Headers.Add("Application-Name", "QMod-Manager-Reloaded");
        modUpdate.Headers.Add("User-Agent", $"QMod-Manager-Reloaded/{Assembly.GetExecutingAssembly().GetName().Version} {Environment.OSVersion}");

        modUpdate.DownloadStringAsync(new Uri($"https://api.nexusmods.com/v1/games/graveyardkeeper/mods/{mod.NexusId}/files.json"));

        void UpdateFilesDownloadedCompleted(object sender, DownloadStringCompletedEventArgs args)
        {
            LblNexusRequests.Text = UpdateRequestCounts(modUpdate.ResponseHeaders, _settings.UserName, _settings.IsPremium);

            var nexusModFiles = JsonSerializer.Deserialize<NexusModFiles>(args.Result);
            if (nexusModFiles == null) return;
            var updateFile = nexusModFiles.FileUpdates.Last().NewFileId;
            var apiKey = Obscure.Decrypt(_settings.ApiKey, pairedKeys.Lock, pairedKeys.Vector);

            var fileUpdate = new WebClient();
            fileUpdate.DownloadStringCompleted += DownloadedCompleted;
            fileUpdate.Headers.Add("apikey", apiKey);
            fileUpdate.Headers.Add("Application-Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            fileUpdate.Headers.Add("Application-Name", "QMod-Manager-Reloaded");
            fileUpdate.Headers.Add("User-Agent",
                $"QMod-Manager-Reloaded/{Assembly.GetExecutingAssembly().GetName().Version} {Environment.OSVersion}");

            fileUpdate.DownloadStringAsync(new Uri($"https://api.nexusmods.com/v1/games/graveyardkeeper/mods/{mod.NexusId}/files/{updateFile}/download_link.json"));

            void DownloadedCompleted(object sender, DownloadStringCompletedEventArgs innerArgs)
            {
                var downloadLocation = JsonSerializer.Deserialize<List<NexusModFilesServer>>(innerArgs.Result);
                if (downloadLocation is not { Count: > 0 }) return;

                var location = downloadLocation.FirstOrDefault(x => x.ShortName == "Nexus CDN");
                ProcessDownloadLink(mod, location?.Uri, apiKey, nexusModFiles.FileUpdates.Last().NewFileName);
            }
            fileUpdate.Dispose();
        }
        modUpdate.Dispose();
    }

    private void ModMenuUpdate_Click(object sender, EventArgs e)
    {
        modListCtxMenu.Close();
        StartUpdates(_contextMenuMod);
    }

    private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _frmOptions ??= new FrmOptions(ref _settings);
        _frmOptions.ShowDialog();
        _frmOptions = null;
    }

    private void UpdateAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
        modListCtxMenu.Close();
        foreach (var mod in _modList.Where(x => x.UpdateAvailable))
        {
            StartUpdates(mod);
        }
    }
}