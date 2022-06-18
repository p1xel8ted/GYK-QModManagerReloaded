using System;
using System.Windows.Forms;

namespace QModReloadedGUI
{
    public partial class FrmNexus : Form
    {
        public FrmNexus()
        {
            InitializeComponent();
        }

        private void FrmNexus_Load(object sender, EventArgs e)
        {
           TxtApi.Text = Properties.Settings.Default.API.Trim();
        }

        private void FrmNexus_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.API = TxtApi.Text.Trim();
            Properties.Settings.Default.Save();
        }
    }
}
