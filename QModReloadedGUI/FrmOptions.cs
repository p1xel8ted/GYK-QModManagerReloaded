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
            ChkDisabledAtBottom.Checked = _settings.DisabledModsAtBottom;
        }

        private void ChkUpdateOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            ChkUpdateOnStartup.Checked = _settings.UpdateOnStartup;
            _settings.Save();
        }

        private void ChkLaunchExeDirectly_CheckedChanged(object sender, EventArgs e)
        {
            ChkLaunchExeDirectly.Checked = _settings.LaunchDirectly;
            _settings.Save();
        }

        private void ChkDisabledAtBottom_CheckedChanged(object sender, EventArgs e)
        {
            ChkDisabledAtBottom.Checked = _settings.DisabledModsAtBottom;
            _settings.Save();
        }
    }
}
