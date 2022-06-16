using System.ComponentModel;
using System.Windows.Forms;

namespace QModReloadedGUI
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.TxtGameLocation = new System.Windows.Forms.TextBox();
            this.TxtModFolderLocation = new System.Windows.Forms.TextBox();
            this.LblGameLocation = new System.Windows.Forms.Label();
            this.LblModFolderLocation = new System.Windows.Forms.Label();
            this.LblInstalledMods = new System.Windows.Forms.Label();
            this.LblModInfo = new System.Windows.Forms.Label();
            this.TxtModInfo = new System.Windows.Forms.TextBox();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.DgvMods = new System.Windows.Forms.DataGridView();
            this.ChOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChMod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ChID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BtnRestore = new System.Windows.Forms.Button();
            this.BtnOpenLog = new System.Windows.Forms.Button();
            this.BtnOpenModDir = new System.Windows.Forms.Button();
            this.BtnOpenGameDir = new System.Windows.Forms.Button();
            this.BtnRemoveIntros = new System.Windows.Forms.Button();
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.BtnRemovePatch = new System.Windows.Forms.Button();
            this.BtnRunGame = new System.Windows.Forms.PictureBox();
            this.BtnRemove = new System.Windows.Forms.Button();
            this.BtnAddMod = new System.Windows.Forms.Button();
            this.BtnPatch = new System.Windows.Forms.Button();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checklistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.LblPatched = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.LblIntroPatched = new System.Windows.Forms.ToolStripLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.checklistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyResolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnLaunchModless = new System.Windows.Forms.ToolStripMenuItem();
            this.DlgFile = new System.Windows.Forms.OpenFileDialog();
            this.LblConfig = new System.Windows.Forms.Label();
            this.TxtConfig = new System.Windows.Forms.TextBox();
            this.LblSaved = new System.Windows.Forms.Label();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIconCtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.launchGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openmModDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGameDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ChkToggleMods = new System.Windows.Forms.CheckBox();
            this.ChkLaunchExeDirectly = new System.Windows.Forms.CheckBox();
            this.DgvLog = new System.Windows.Forms.DataGridView();
            this.ChTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LblErrors = new System.Windows.Forms.Label();
            this.openUnityLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.openSaveDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.DgvMods)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRunGame)).BeginInit();
            this.ToolStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.trayIconCtxMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvLog)).BeginInit();
            this.SuspendLayout();
            // 
            // TxtGameLocation
            // 
            this.TxtGameLocation.Location = new System.Drawing.Point(12, 29);
            this.TxtGameLocation.Name = "TxtGameLocation";
            this.TxtGameLocation.ReadOnly = true;
            this.TxtGameLocation.Size = new System.Drawing.Size(637, 20);
            this.TxtGameLocation.TabIndex = 0;
            this.ToolTip.SetToolTip(this.TxtGameLocation, "Directory that contains Graveyard Keeper.exe");
            // 
            // TxtModFolderLocation
            // 
            this.TxtModFolderLocation.Location = new System.Drawing.Point(12, 84);
            this.TxtModFolderLocation.Name = "TxtModFolderLocation";
            this.TxtModFolderLocation.ReadOnly = true;
            this.TxtModFolderLocation.Size = new System.Drawing.Size(496, 20);
            this.TxtModFolderLocation.TabIndex = 1;
            this.ToolTip.SetToolTip(this.TxtModFolderLocation, "This cannot be changed due to the nature of QMods.");
            // 
            // LblGameLocation
            // 
            this.LblGameLocation.AutoSize = true;
            this.LblGameLocation.Location = new System.Drawing.Point(12, 13);
            this.LblGameLocation.Name = "LblGameLocation";
            this.LblGameLocation.Size = new System.Drawing.Size(81, 17);
            this.LblGameLocation.TabIndex = 3;
            this.LblGameLocation.Text = "Game Location";
            this.LblGameLocation.UseCompatibleTextRendering = true;
            // 
            // LblModFolderLocation
            // 
            this.LblModFolderLocation.AutoSize = true;
            this.LblModFolderLocation.Location = new System.Drawing.Point(12, 64);
            this.LblModFolderLocation.Name = "LblModFolderLocation";
            this.LblModFolderLocation.Size = new System.Drawing.Size(107, 17);
            this.LblModFolderLocation.TabIndex = 4;
            this.LblModFolderLocation.Text = "Mod Folder Location";
            this.LblModFolderLocation.UseCompatibleTextRendering = true;
            // 
            // LblInstalledMods
            // 
            this.LblInstalledMods.AutoSize = true;
            this.LblInstalledMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblInstalledMods.Location = new System.Drawing.Point(12, 120);
            this.LblInstalledMods.Name = "LblInstalledMods";
            this.LblInstalledMods.Size = new System.Drawing.Size(77, 17);
            this.LblInstalledMods.TabIndex = 5;
            this.LblInstalledMods.Text = "Installed Mods";
            this.LblInstalledMods.UseCompatibleTextRendering = true;
            // 
            // LblModInfo
            // 
            this.LblModInfo.AutoSize = true;
            this.LblModInfo.Location = new System.Drawing.Point(382, 120);
            this.LblModInfo.Name = "LblModInfo";
            this.LblModInfo.Size = new System.Drawing.Size(48, 17);
            this.LblModInfo.TabIndex = 9;
            this.LblModInfo.Text = "Mod Info";
            this.LblModInfo.UseCompatibleTextRendering = true;
            // 
            // TxtModInfo
            // 
            this.TxtModInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtModInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtModInfo.Location = new System.Drawing.Point(384, 140);
            this.TxtModInfo.Multiline = true;
            this.TxtModInfo.Name = "TxtModInfo";
            this.TxtModInfo.ReadOnly = true;
            this.TxtModInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtModInfo.Size = new System.Drawing.Size(346, 180);
            this.TxtModInfo.TabIndex = 11;
            // 
            // DgvMods
            // 
            this.DgvMods.AllowDrop = true;
            this.DgvMods.AllowUserToAddRows = false;
            this.DgvMods.AllowUserToDeleteRows = false;
            this.DgvMods.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvMods.CausesValidation = false;
            this.DgvMods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvMods.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChOrder,
            this.ChMod,
            this.ChEnabled,
            this.ChID});
            this.DgvMods.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DgvMods.GridColor = System.Drawing.SystemColors.Control;
            this.DgvMods.Location = new System.Drawing.Point(12, 140);
            this.DgvMods.Name = "DgvMods";
            this.DgvMods.ReadOnly = true;
            this.DgvMods.RowHeadersVisible = false;
            this.DgvMods.ShowEditingIcon = false;
            this.DgvMods.Size = new System.Drawing.Size(364, 383);
            this.DgvMods.TabIndex = 35;
            this.ToolTip.SetToolTip(this.DgvMods, "Drag n Drop to re-order mods. Mods will load in the order they appear in this lis" +
        "t.");
            this.DgvMods.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvMods_CellClick);
            this.DgvMods.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvMods_CellContentClick);
            this.DgvMods.DragDrop += new System.Windows.Forms.DragEventHandler(this.DgvMods_DragDrop);
            this.DgvMods.DragOver += new System.Windows.Forms.DragEventHandler(this.DgvMods_DragOver);
            this.DgvMods.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DgvMods_KeyUp);
            this.DgvMods.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DgvMods_MouseDoubleClick);
            this.DgvMods.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DgvMods_MouseDown);
            this.DgvMods.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DgvMods_MouseMove);
            // 
            // ChOrder
            // 
            this.ChOrder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ChOrder.HeaderText = "Order";
            this.ChOrder.Name = "ChOrder";
            this.ChOrder.ReadOnly = true;
            this.ChOrder.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ChOrder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ChOrder.Width = 58;
            // 
            // ChMod
            // 
            this.ChMod.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ChMod.HeaderText = "Mod";
            this.ChMod.Name = "ChMod";
            this.ChMod.ReadOnly = true;
            this.ChMod.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ChEnabled
            // 
            this.ChEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ChEnabled.HeaderText = "Enabled";
            this.ChEnabled.Name = "ChEnabled";
            this.ChEnabled.ReadOnly = true;
            this.ChEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ChEnabled.Width = 52;
            // 
            // ChID
            // 
            this.ChID.HeaderText = "ID";
            this.ChID.Name = "ChID";
            this.ChID.ReadOnly = true;
            this.ChID.Visible = false;
            // 
            // BtnRestore
            // 
            this.BtnRestore.Image = global::QModReloadedGUI.Properties.Resources.save;
            this.BtnRestore.Location = new System.Drawing.Point(487, 529);
            this.BtnRestore.Name = "BtnRestore";
            this.BtnRestore.Size = new System.Drawing.Size(120, 25);
            this.BtnRestore.TabIndex = 36;
            this.BtnRestore.Text = "Restore Backup";
            this.BtnRestore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnRestore.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnRestore, "Restores backed up Assembly-CSharp.dll if it exists.");
            this.BtnRestore.UseCompatibleTextRendering = true;
            this.BtnRestore.UseVisualStyleBackColor = true;
            this.BtnRestore.Click += new System.EventHandler(this.BtnRestore_Click);
            // 
            // BtnOpenLog
            // 
            this.BtnOpenLog.Image = global::QModReloadedGUI.Properties.Resources.comments;
            this.BtnOpenLog.Location = new System.Drawing.Point(396, 529);
            this.BtnOpenLog.Name = "BtnOpenLog";
            this.BtnOpenLog.Size = new System.Drawing.Size(85, 25);
            this.BtnOpenLog.TabIndex = 29;
            this.BtnOpenLog.Text = "Open &Log";
            this.BtnOpenLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnOpenLog.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnOpenLog, "Open the log file in your default editor.");
            this.BtnOpenLog.UseVisualStyleBackColor = true;
            this.BtnOpenLog.Click += new System.EventHandler(this.BtnOpenLog_Click);
            // 
            // BtnOpenModDir
            // 
            this.BtnOpenModDir.Image = global::QModReloadedGUI.Properties.Resources.folder_files;
            this.BtnOpenModDir.Location = new System.Drawing.Point(499, 110);
            this.BtnOpenModDir.Name = "BtnOpenModDir";
            this.BtnOpenModDir.Size = new System.Drawing.Size(108, 25);
            this.BtnOpenModDir.TabIndex = 28;
            this.BtnOpenModDir.Text = "Open M&od Dir";
            this.BtnOpenModDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnOpenModDir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnOpenModDir, "Open the mod directory in Explorer");
            this.BtnOpenModDir.UseCompatibleTextRendering = true;
            this.BtnOpenModDir.UseVisualStyleBackColor = true;
            this.BtnOpenModDir.Click += new System.EventHandler(this.BtnOpenModDir_Click);
            // 
            // BtnOpenGameDir
            // 
            this.BtnOpenGameDir.Image = global::QModReloadedGUI.Properties.Resources.folder_open;
            this.BtnOpenGameDir.Location = new System.Drawing.Point(613, 110);
            this.BtnOpenGameDir.Name = "BtnOpenGameDir";
            this.BtnOpenGameDir.Size = new System.Drawing.Size(121, 25);
            this.BtnOpenGameDir.TabIndex = 27;
            this.BtnOpenGameDir.Text = "Ope&n Game Dir";
            this.BtnOpenGameDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnOpenGameDir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnOpenGameDir, "Open the game directory in Explorer");
            this.BtnOpenGameDir.UseVisualStyleBackColor = true;
            this.BtnOpenGameDir.Click += new System.EventHandler(this.BtnOpenGameDir_Click);
            // 
            // BtnRemoveIntros
            // 
            this.BtnRemoveIntros.Image = global::QModReloadedGUI.Properties.Resources.application;
            this.BtnRemoveIntros.Location = new System.Drawing.Point(613, 529);
            this.BtnRemoveIntros.Name = "BtnRemoveIntros";
            this.BtnRemoveIntros.Size = new System.Drawing.Size(117, 25);
            this.BtnRemoveIntros.TabIndex = 20;
            this.BtnRemoveIntros.Text = "Remove &Intros";
            this.BtnRemoveIntros.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnRemoveIntros.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnRemoveIntros, "Removes intros (permanently).");
            this.BtnRemoveIntros.UseCompatibleTextRendering = true;
            this.BtnRemoveIntros.UseVisualStyleBackColor = true;
            this.BtnRemoveIntros.Click += new System.EventHandler(this.BtnRemoveIntros_Click);
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Image = global::QModReloadedGUI.Properties.Resources.search;
            this.BtnRefresh.Location = new System.Drawing.Point(201, 529);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(86, 25);
            this.BtnRefresh.TabIndex = 18;
            this.BtnRefresh.Text = "Re&fresh";
            this.BtnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnRefresh, "Click if you\'ve installed mods externally.");
            this.BtnRefresh.UseCompatibleTextRendering = true;
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // BtnRemovePatch
            // 
            this.BtnRemovePatch.Image = global::QModReloadedGUI.Properties.Resources.minimize;
            this.BtnRemovePatch.Location = new System.Drawing.Point(539, 53);
            this.BtnRemovePatch.Margin = new System.Windows.Forms.Padding(1);
            this.BtnRemovePatch.Name = "BtnRemovePatch";
            this.BtnRemovePatch.Size = new System.Drawing.Size(110, 25);
            this.BtnRemovePatch.TabIndex = 17;
            this.BtnRemovePatch.Text = "Remove Pa&tch";
            this.BtnRemovePatch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnRemovePatch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnRemovePatch, "Removes the mod patch only.");
            this.BtnRemovePatch.UseCompatibleTextRendering = true;
            this.BtnRemovePatch.UseVisualStyleBackColor = true;
            this.BtnRemovePatch.Click += new System.EventHandler(this.BtnRemovePatch_Click);
            // 
            // BtnRunGame
            // 
            this.BtnRunGame.Image = ((System.Drawing.Image)(resources.GetObject("BtnRunGame.Image")));
            this.BtnRunGame.Location = new System.Drawing.Point(655, 29);
            this.BtnRunGame.Name = "BtnRunGame";
            this.BtnRunGame.Size = new System.Drawing.Size(75, 75);
            this.BtnRunGame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.BtnRunGame.TabIndex = 16;
            this.BtnRunGame.TabStop = false;
            this.ToolTip.SetToolTip(this.BtnRunGame, "Click to launch Graveyard Keeper. Launches via Steam first, then by the EXE direc" +
        "tly if Steam fails for whatever reason.");
            this.BtnRunGame.Click += new System.EventHandler(this.BtnRunGame_Click);
            // 
            // BtnRemove
            // 
            this.BtnRemove.Image = global::QModReloadedGUI.Properties.Resources.action_delete;
            this.BtnRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnRemove.Location = new System.Drawing.Point(95, 529);
            this.BtnRemove.Name = "BtnRemove";
            this.BtnRemove.Size = new System.Drawing.Size(100, 25);
            this.BtnRemove.TabIndex = 14;
            this.BtnRemove.Text = "&Remove Mod";
            this.BtnRemove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnRemove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnRemove, "Removes the selected mod(s).");
            this.BtnRemove.UseCompatibleTextRendering = true;
            this.BtnRemove.UseVisualStyleBackColor = true;
            this.BtnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // BtnAddMod
            // 
            this.BtnAddMod.Image = global::QModReloadedGUI.Properties.Resources.action_add;
            this.BtnAddMod.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnAddMod.Location = new System.Drawing.Point(12, 529);
            this.BtnAddMod.Name = "BtnAddMod";
            this.BtnAddMod.Size = new System.Drawing.Size(77, 25);
            this.BtnAddMod.TabIndex = 13;
            this.BtnAddMod.Text = "A&dd Mod";
            this.BtnAddMod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnAddMod.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnAddMod, "Adds a new mod.");
            this.BtnAddMod.UseCompatibleTextRendering = true;
            this.BtnAddMod.UseVisualStyleBackColor = true;
            this.BtnAddMod.Click += new System.EventHandler(this.BtnAddMod_Click);
            // 
            // BtnPatch
            // 
            this.BtnPatch.Image = global::QModReloadedGUI.Properties.Resources.maximize;
            this.BtnPatch.Location = new System.Drawing.Point(439, 53);
            this.BtnPatch.Margin = new System.Windows.Forms.Padding(1);
            this.BtnPatch.Name = "BtnPatch";
            this.BtnPatch.Size = new System.Drawing.Size(98, 25);
            this.BtnPatch.TabIndex = 7;
            this.BtnPatch.Text = "&Apply Patch";
            this.BtnPatch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnPatch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ToolTip.SetToolTip(this.BtnPatch, "Applies the mod patch.");
            this.BtnPatch.UseCompatibleTextRendering = true;
            this.BtnPatch.UseVisualStyleBackColor = true;
            this.BtnPatch.Click += new System.EventHandler(this.BtnPatch_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            // 
            // checklistToolStripMenuItem
            // 
            this.checklistToolStripMenuItem.Name = "checklistToolStripMenuItem";
            this.checklistToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "&About";
            // 
            // ToolStrip
            // 
            this.ToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LblPatched,
            this.toolStripSeparator2,
            this.LblIntroPatched});
            this.ToolStrip.Location = new System.Drawing.Point(0, 759);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(742, 25);
            this.ToolStrip.TabIndex = 25;
            this.ToolStrip.Text = "toolStrip1";
            // 
            // LblPatched
            // 
            this.LblPatched.Name = "LblPatched";
            this.LblPatched.Size = new System.Drawing.Size(86, 22);
            this.LblPatched.Text = "toolStripLabel1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // LblIntroPatched
            // 
            this.LblIntroPatched.Name = "LblIntroPatched";
            this.LblIntroPatched.Size = new System.Drawing.Size(86, 22);
            this.LblIntroPatched.Text = "toolStripLabel2";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.checklistToolStripMenuItem1,
            this.modifyResolutionToolStripMenuItem,
            this.aboutToolStripMenuItem1,
            this.BtnLaunchModless});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(742, 24);
            this.menuStrip1.TabIndex = 26;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSaveDirectoryToolStripMenuItem,
            this.openUnityLogToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem1});
            this.fileToolStripMenuItem1.Image = global::QModReloadedGUI.Properties.Resources.arrow_down;
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(53, 20);
            this.fileToolStripMenuItem1.Text = "F&ile";
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Image = global::QModReloadedGUI.Properties.Resources.stop;
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem1.Text = "E&xit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.ExitToolStripMenuItem1_Click);
            // 
            // checklistToolStripMenuItem1
            // 
            this.checklistToolStripMenuItem1.Image = global::QModReloadedGUI.Properties.Resources.action_check;
            this.checklistToolStripMenuItem1.Name = "checklistToolStripMenuItem1";
            this.checklistToolStripMenuItem1.Size = new System.Drawing.Size(83, 20);
            this.checklistToolStripMenuItem1.Text = "C&hecklist";
            this.checklistToolStripMenuItem1.ToolTipText = "Click to see if your installation is valid for mods to function.";
            this.checklistToolStripMenuItem1.Click += new System.EventHandler(this.ChecklistToolStripMenuItem1_Click);
            // 
            // modifyResolutionToolStripMenuItem
            // 
            this.modifyResolutionToolStripMenuItem.Image = global::QModReloadedGUI.Properties.Resources.login;
            this.modifyResolutionToolStripMenuItem.Name = "modifyResolutionToolStripMenuItem";
            this.modifyResolutionToolStripMenuItem.Size = new System.Drawing.Size(132, 20);
            this.modifyResolutionToolStripMenuItem.Text = "&Modify Resolution";
            this.modifyResolutionToolStripMenuItem.Click += new System.EventHandler(this.ModifyResolutionToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Image = global::QModReloadedGUI.Properties.Resources.file;
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(68, 20);
            this.aboutToolStripMenuItem1.Text = "A&bout";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.AboutToolStripMenuItem1_Click);
            // 
            // BtnLaunchModless
            // 
            this.BtnLaunchModless.Image = global::QModReloadedGUI.Properties.Resources.play;
            this.BtnLaunchModless.Name = "BtnLaunchModless";
            this.BtnLaunchModless.Size = new System.Drawing.Size(121, 20);
            this.BtnLaunchModless.Text = "&Launch Modless";
            this.BtnLaunchModless.Click += new System.EventHandler(this.BtnLaunchModless_Click);
            // 
            // DlgFile
            // 
            this.DlgFile.Filter = "ZIP Files|*.zip";
            this.DlgFile.Multiselect = true;
            this.DlgFile.Title = "Select ZIP file( s)";
            // 
            // LblConfig
            // 
            this.LblConfig.AutoSize = true;
            this.LblConfig.Location = new System.Drawing.Point(382, 325);
            this.LblConfig.Name = "LblConfig";
            this.LblConfig.Size = new System.Drawing.Size(37, 17);
            this.LblConfig.TabIndex = 30;
            this.LblConfig.Text = "Config";
            this.LblConfig.UseCompatibleTextRendering = true;
            // 
            // TxtConfig
            // 
            this.TxtConfig.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtConfig.Location = new System.Drawing.Point(382, 345);
            this.TxtConfig.Multiline = true;
            this.TxtConfig.Name = "TxtConfig";
            this.TxtConfig.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtConfig.Size = new System.Drawing.Size(348, 178);
            this.TxtConfig.TabIndex = 31;
            this.TxtConfig.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtConfig_KeyUp);
            this.TxtConfig.Leave += new System.EventHandler(this.TxtConfig_Leave);
            // 
            // LblSaved
            // 
            this.LblSaved.AutoSize = true;
            this.LblSaved.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblSaved.ForeColor = System.Drawing.Color.Green;
            this.LblSaved.Location = new System.Drawing.Point(425, 325);
            this.LblSaved.Name = "LblSaved";
            this.LblSaved.Size = new System.Drawing.Size(46, 17);
            this.LblSaved.TabIndex = 32;
            this.LblSaved.Text = "Saved...";
            this.LblSaved.UseCompatibleTextRendering = true;
            this.LblSaved.Visible = false;
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.BalloonTipTitle = "QMod Manager Reloaded";
            this.trayIcon.ContextMenuStrip = this.trayIconCtxMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "QMod Manager Reloaded";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseDoubleClick);
            // 
            // trayIconCtxMenu
            // 
            this.trayIconCtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreWindowToolStripMenuItem,
            this.toolStripMenuItem2,
            this.launchGameToolStripMenuItem,
            this.openmModDirectoryToolStripMenuItem,
            this.openGameDirectoryToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem2});
            this.trayIconCtxMenu.Name = "trayIconCtxMenu";
            this.trayIconCtxMenu.Size = new System.Drawing.Size(189, 126);
            // 
            // restoreWindowToolStripMenuItem
            // 
            this.restoreWindowToolStripMenuItem.Image = global::QModReloadedGUI.Properties.Resources.arrow_top;
            this.restoreWindowToolStripMenuItem.Name = "restoreWindowToolStripMenuItem";
            this.restoreWindowToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.restoreWindowToolStripMenuItem.Text = "&Restore Window";
            this.restoreWindowToolStripMenuItem.Click += new System.EventHandler(this.RestoreWindowToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(185, 6);
            // 
            // launchGameToolStripMenuItem
            // 
            this.launchGameToolStripMenuItem.Image = global::QModReloadedGUI.Properties.Resources.play;
            this.launchGameToolStripMenuItem.Name = "launchGameToolStripMenuItem";
            this.launchGameToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.launchGameToolStripMenuItem.Text = "&Launch Game";
            this.launchGameToolStripMenuItem.Click += new System.EventHandler(this.LaunchGameToolStripMenuItem_Click);
            // 
            // openmModDirectoryToolStripMenuItem
            // 
            this.openmModDirectoryToolStripMenuItem.Image = global::QModReloadedGUI.Properties.Resources.folder_files1;
            this.openmModDirectoryToolStripMenuItem.Name = "openmModDirectoryToolStripMenuItem";
            this.openmModDirectoryToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.openmModDirectoryToolStripMenuItem.Text = "Open &Mod Directory";
            this.openmModDirectoryToolStripMenuItem.Click += new System.EventHandler(this.OpenmModDirectoryToolStripMenuItem_Click);
            // 
            // openGameDirectoryToolStripMenuItem
            // 
            this.openGameDirectoryToolStripMenuItem.Image = global::QModReloadedGUI.Properties.Resources.folder_open1;
            this.openGameDirectoryToolStripMenuItem.Name = "openGameDirectoryToolStripMenuItem";
            this.openGameDirectoryToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.openGameDirectoryToolStripMenuItem.Text = "Open &Game Directory";
            this.openGameDirectoryToolStripMenuItem.Click += new System.EventHandler(this.OpenGameDirectoryToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(185, 6);
            // 
            // exitToolStripMenuItem2
            // 
            this.exitToolStripMenuItem2.Image = global::QModReloadedGUI.Properties.Resources.stop;
            this.exitToolStripMenuItem2.Name = "exitToolStripMenuItem2";
            this.exitToolStripMenuItem2.Size = new System.Drawing.Size(188, 22);
            this.exitToolStripMenuItem2.Text = "E&xit";
            this.exitToolStripMenuItem2.Click += new System.EventHandler(this.ExitToolStripMenuItem2_Click);
            // 
            // ChkToggleMods
            // 
            this.ChkToggleMods.AutoSize = true;
            this.ChkToggleMods.Location = new System.Drawing.Point(325, 120);
            this.ChkToggleMods.Name = "ChkToggleMods";
            this.ChkToggleMods.Size = new System.Drawing.Size(15, 14);
            this.ChkToggleMods.TabIndex = 37;
            this.ChkToggleMods.UseVisualStyleBackColor = true;
            this.ChkToggleMods.Click += new System.EventHandler(this.ChkToggleMods_Click);
            // 
            // ChkLaunchExeDirectly
            // 
            this.ChkLaunchExeDirectly.AutoSize = true;
            this.ChkLaunchExeDirectly.Location = new System.Drawing.Point(514, 84);
            this.ChkLaunchExeDirectly.Name = "ChkLaunchExeDirectly";
            this.ChkLaunchExeDirectly.Size = new System.Drawing.Size(135, 18);
            this.ChkLaunchExeDirectly.TabIndex = 38;
            this.ChkLaunchExeDirectly.Text = "Launch Game Directly";
            this.ChkLaunchExeDirectly.UseCompatibleTextRendering = true;
            this.ChkLaunchExeDirectly.UseVisualStyleBackColor = true;
            this.ChkLaunchExeDirectly.CheckStateChanged += new System.EventHandler(this.ChkLaunchExeDirectly_CheckStateChanged);
            // 
            // DgvLog
            // 
            this.DgvLog.AllowDrop = true;
            this.DgvLog.AllowUserToAddRows = false;
            this.DgvLog.AllowUserToDeleteRows = false;
            this.DgvLog.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvLog.CausesValidation = false;
            this.DgvLog.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.DgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChTime,
            this.dataGridViewTextBoxColumn1});
            this.DgvLog.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DgvLog.GridColor = System.Drawing.SystemColors.Control;
            this.DgvLog.Location = new System.Drawing.Point(12, 560);
            this.DgvLog.MultiSelect = false;
            this.DgvLog.Name = "DgvLog";
            this.DgvLog.ReadOnly = true;
            this.DgvLog.RowHeadersVisible = false;
            this.DgvLog.ShowEditingIcon = false;
            this.DgvLog.Size = new System.Drawing.Size(718, 186);
            this.DgvLog.TabIndex = 39;
            // 
            // ChTime
            // 
            this.ChTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ChTime.HeaderText = "Time";
            this.ChTime.Name = "ChTime";
            this.ChTime.ReadOnly = true;
            this.ChTime.Width = 55;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "Log";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // LblErrors
            // 
            this.LblErrors.AutoSize = true;
            this.LblErrors.ForeColor = System.Drawing.Color.Red;
            this.LblErrors.Location = new System.Drawing.Point(293, 535);
            this.LblErrors.Name = "LblErrors";
            this.LblErrors.Size = new System.Drawing.Size(69, 13);
            this.LblErrors.TabIndex = 40;
            this.LblErrors.Text = "Cant See Me";
            // 
            // openUnityLogToolStripMenuItem
            // 
            this.openUnityLogToolStripMenuItem.Image = global::QModReloadedGUI.Properties.Resources.comments;
            this.openUnityLogToolStripMenuItem.Name = "openUnityLogToolStripMenuItem";
            this.openUnityLogToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openUnityLogToolStripMenuItem.Text = "&Open Unity Log";
            this.openUnityLogToolStripMenuItem.Click += new System.EventHandler(this.openUnityLogToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(177, 6);
            // 
            // openSaveDirectoryToolStripMenuItem
            // 
            this.openSaveDirectoryToolStripMenuItem.Image = global::QModReloadedGUI.Properties.Resources.folder_files;
            this.openSaveDirectoryToolStripMenuItem.Name = "openSaveDirectoryToolStripMenuItem";
            this.openSaveDirectoryToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.openSaveDirectoryToolStripMenuItem.Text = "Open &Save Directory";
            this.openSaveDirectoryToolStripMenuItem.Click += new System.EventHandler(this.openSaveDirectoryToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 784);
            this.Controls.Add(this.LblErrors);
            this.Controls.Add(this.DgvLog);
            this.Controls.Add(this.ChkLaunchExeDirectly);
            this.Controls.Add(this.ChkToggleMods);
            this.Controls.Add(this.BtnRestore);
            this.Controls.Add(this.DgvMods);
            this.Controls.Add(this.LblSaved);
            this.Controls.Add(this.TxtConfig);
            this.Controls.Add(this.LblConfig);
            this.Controls.Add(this.BtnOpenLog);
            this.Controls.Add(this.BtnOpenModDir);
            this.Controls.Add(this.BtnOpenGameDir);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.BtnRemoveIntros);
            this.Controls.Add(this.BtnRefresh);
            this.Controls.Add(this.BtnRemovePatch);
            this.Controls.Add(this.BtnRunGame);
            this.Controls.Add(this.BtnRemove);
            this.Controls.Add(this.BtnAddMod);
            this.Controls.Add(this.TxtModInfo);
            this.Controls.Add(this.LblModInfo);
            this.Controls.Add(this.BtnPatch);
            this.Controls.Add(this.LblInstalledMods);
            this.Controls.Add(this.LblModFolderLocation);
            this.Controls.Add(this.LblGameLocation);
            this.Controls.Add(this.TxtModFolderLocation);
            this.Controls.Add(this.TxtGameLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QMod Manager Reloaded";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.DgvMods)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRunGame)).EndInit();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.trayIconCtxMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox TxtGameLocation;
        private TextBox TxtModFolderLocation;
        private Label LblGameLocation;
        private Label LblModFolderLocation;
        private Label LblInstalledMods;
        private Button BtnPatch;
        private Label LblModInfo;
        private TextBox TxtModInfo;
        private Button BtnAddMod;
        private Button BtnRemove;
        private PictureBox BtnRunGame;
        private ToolTip ToolTip;
        private Button BtnRemovePatch;
        private Button BtnRefresh;
        private Button BtnRemoveIntros;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem checklistToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStrip ToolStrip;
        private ToolStripLabel LblPatched;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripLabel LblIntroPatched;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem1;
        private ToolStripMenuItem checklistToolStripMenuItem1;
        private ToolStripMenuItem aboutToolStripMenuItem1;
        private OpenFileDialog DlgFile;
        private Button BtnOpenGameDir;
        private Button BtnOpenModDir;
        private Button BtnOpenLog;
        private ToolStripMenuItem modifyResolutionToolStripMenuItem;
        private Label LblConfig;
        private TextBox TxtConfig;
        private Label LblSaved;
        private DataGridView DgvMods;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayIconCtxMenu;
        private ToolStripMenuItem restoreWindowToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem launchGameToolStripMenuItem;
        private ToolStripMenuItem openmModDirectoryToolStripMenuItem;
        private ToolStripMenuItem openGameDirectoryToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem2;
        private Button BtnRestore;
        private CheckBox ChkToggleMods;
        private ToolStripMenuItem BtnLaunchModless;
        private CheckBox ChkLaunchExeDirectly;
        public DataGridView DgvLog;
        private DataGridViewTextBoxColumn ChTime;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private Label LblErrors;
        private DataGridViewTextBoxColumn ChOrder;
        private DataGridViewTextBoxColumn ChMod;
        private DataGridViewCheckBoxColumn ChEnabled;
        private DataGridViewTextBoxColumn ChID;
        private ToolStripMenuItem openUnityLogToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem openSaveDirectoryToolStripMenuItem;
    }
}

