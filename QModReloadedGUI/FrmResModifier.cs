using System;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using Mono.Cecil;
using Mono.Cecil.Cil;
using OpCodes = Mono.Cecil.Cil.OpCodes;


namespace QModReloadedGUI;

public partial class FrmResModifier : Form
{
    private static string _gameLocation;
    private static AssemblyDefinition _resAssembly;
    private static ILProcessor _prc;

    public FrmResModifier(string gameLocation)
    {
        _gameLocation = gameLocation;
        InitializeComponent();
    }

    private void ResModifier_Load(object sender, EventArgs e)
    { 
        try
        {
            _resAssembly = AssemblyDefinition.ReadAssembly(Path.Combine(_gameLocation,
                "Graveyard Keeper_Data\\Managed\\Assembly-CSharp.dll"));
            var toInspect = _resAssembly.MainModule
                .GetTypes()
                .SelectMany(t => t.Methods
                    .Where(m => m.HasBody)
                    .Select(m => new { t, m }));

            toInspect = toInspect.Where(x =>
                x.t.Name.EndsWith("ResolutionConfig") && x.m.Name == "GetResolutionConfigOrNull");
            _prc = toInspect.FirstOrDefault()?.m.Body.GetILProcessor();
            GetCurrentResolutions();
        }
        catch (Exception ex)
        {
            Utilities.WriteLog($"Res Modifier (Load): ERROR: {ex.Message}", _gameLocation);
        }
    }

    private void UpdateResolutions()
    {
        try
        {
            var newHeight = _prc?.Create(OpCodes.Ldc_I4, int.Parse(TxtRequestedMaxHeight.Text));
            var newWidth = _prc?.Create(OpCodes.Ldc_I4, int.Parse(TxtRequestedMaxWidth.Text));

            _prc?.Replace(_prc?.Body.Instructions[19], newHeight);
            _prc?.Replace(_prc?.Body.Instructions[22], newWidth);

            _resAssembly.Write(Path.Combine(_gameLocation, "Graveyard Keeper_Data\\Managed\\Assembly-CSharp.dll"));
            Utilities.WriteLog("Res Modifier (Update Resolutions): Successfully patched in new resolution.", _gameLocation);
        }
        catch (Exception ex)
        {
            Utilities.WriteLog($"Res Modifier (Update Resolutions): ERROR: {ex.Message}", _gameLocation);
        }
    }

    private void GetCurrentResolutions()
    {
        try
        {
            var currentHeight = Convert.ToInt32(_prc?.Body.Instructions[19].Operand.ToString());
            var currentWidth = Convert.ToInt32(_prc?.Body.Instructions[22].Operand.ToString());
            TxtMaxHeight.Text = currentHeight.ToString();
            TxtMaxWidth.Text = currentWidth.ToString();
            Utilities.WriteLog("Res Modifier (GetCurrentResolutions): Obtained current max resolution.", _gameLocation);
        }
        catch (Exception ex)
        {
            Utilities.WriteLog($"Res Modifier (GetCurrentResolutions): ERROR: {ex.Message}", _gameLocation);
        }
    }

    private void BtnApply_Click(object sender, EventArgs e)
    {
        var isHeightNumber = int.TryParse(TxtRequestedMaxHeight.Text, out _);
        var isWidthNumber = int.TryParse(TxtRequestedMaxWidth.Text, out _);
        if (isHeightNumber && isWidthNumber)
        {
            UpdateResolutions();
            GetCurrentResolutions();
        }
        else
        {
            MessageBox.Show(@"Enter integers only.", @"Integers", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}