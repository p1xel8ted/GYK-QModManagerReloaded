using System;
using System.Windows.Forms;

namespace QModReloadedGUI
{
    public partial class FrmOptions : Form
    {
        private readonly Settings _settings;
        public FrmOptions(ref Settings settings)
        {
            _settings = settings;
            InitializeComponent();
        }

        private void FrmOptions_Load(object sender, EventArgs e)
        {
            ChkLaunchExeDirectly.Checked = _settings.LaunchDirectly;
            ChkUpdateOnStartup.Checked = _settings.UpdateOnStartup;
        }

        private void ChkUpdateOnStartup_CheckedChanged(object sender, EventArgs e)
        {
           _settings.UpdateOnStartup = ChkUpdateOnStartup.Checked;
            _settings.Save();
        }

        private void ChkLaunchExeDirectly_CheckedChanged(object sender, EventArgs e)
        {
            _settings.LaunchDirectly = ChkLaunchExeDirectly.Checked;
            _settings.Save();
        }
    }
}
