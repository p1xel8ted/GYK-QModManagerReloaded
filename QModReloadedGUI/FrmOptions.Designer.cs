namespace QModReloadedGUI
{
    partial class FrmOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOptions));
            this.ChkUpdateOnStartup = new System.Windows.Forms.CheckBox();
            this.ChkLaunchExeDirectly = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ChkUpdateOnStartup
            // 
            this.ChkUpdateOnStartup.AutoSize = true;
            this.ChkUpdateOnStartup.Location = new System.Drawing.Point(12, 12);
            this.ChkUpdateOnStartup.Name = "ChkUpdateOnStartup";
            this.ChkUpdateOnStartup.Size = new System.Drawing.Size(163, 17);
            this.ChkUpdateOnStartup.TabIndex = 0;
            this.ChkUpdateOnStartup.Text = "Check for updates on startup";
            this.ChkUpdateOnStartup.UseVisualStyleBackColor = true;
            this.ChkUpdateOnStartup.CheckedChanged += new System.EventHandler(this.ChkUpdateOnStartup_CheckedChanged);
            // 
            // ChkLaunchExeDirectly
            // 
            this.ChkLaunchExeDirectly.AutoSize = true;
            this.ChkLaunchExeDirectly.Location = new System.Drawing.Point(12, 35);
            this.ChkLaunchExeDirectly.Name = "ChkLaunchExeDirectly";
            this.ChkLaunchExeDirectly.Size = new System.Drawing.Size(131, 18);
            this.ChkLaunchExeDirectly.TabIndex = 39;
            this.ChkLaunchExeDirectly.Text = "Launch game directly";
            this.ChkLaunchExeDirectly.UseCompatibleTextRendering = true;
            this.ChkLaunchExeDirectly.UseVisualStyleBackColor = true;
            this.ChkLaunchExeDirectly.CheckedChanged += new System.EventHandler(this.ChkLaunchExeDirectly_CheckedChanged);
            // 
            // FrmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 65);
            this.Controls.Add(this.ChkLaunchExeDirectly);
            this.Controls.Add(this.ChkUpdateOnStartup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmOptions";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.FrmOptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ChkUpdateOnStartup;
        private System.Windows.Forms.CheckBox ChkLaunchExeDirectly;
    }
}