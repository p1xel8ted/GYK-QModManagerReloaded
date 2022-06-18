namespace QModReloadedGUI
{
    partial class FrmNexus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmNexus));
            this.TxtApi = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // TxtApi
            // 
            this.TxtApi.Location = new System.Drawing.Point(12, 12);
            this.TxtApi.Name = "TxtApi";
            this.TxtApi.Size = new System.Drawing.Size(484, 102);
            this.TxtApi.TabIndex = 0;
            this.TxtApi.Text = "";
            // 
            // FrmNexus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 127);
            this.Controls.Add(this.TxtApi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmNexus";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nexus API Key";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmNexus_FormClosing);
            this.Load += new System.EventHandler(this.FrmNexus_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox TxtApi;
    }
}