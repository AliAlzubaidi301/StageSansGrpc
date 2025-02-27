namespace StageCode
{
    partial class Forme1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Forme1));
            MainMenu = new MenuStrip();
            btnFile = new ToolStripMenuItem();
            newToolStripMenuItem1 = new ToolStripMenuItem();
            openToolStripMenuItem1 = new ToolStripMenuItem();
            saveToolStripMenuItem1 = new ToolStripMenuItem();
            saveAsToolStripMenuItem1 = new ToolStripMenuItem();
            btnEdition = new ToolStripMenuItem();
            btnView = new ToolStripMenuItem();
            btnInfos = new ToolStripMenuItem();
            controlCommentToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            btnIP = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            newFormeToolStripMenuItem = new ToolStripMenuItem();
            openFileDialog = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            pnlViewHost = new Panel();
            lstToolbox = new ListBox();
            propertyGrid1 = new PropertyGrid();
            Nouveau = new ToolStripMenuItem();
            menuStrip1 = new MenuStrip();
            Open = new ToolStripMenuItem();
            Save = new ToolStripMenuItem();
            SaveALL = new ToolStripMenuItem();
            Close = new ToolStripMenuItem();
            CloseALL = new ToolStripMenuItem();
            CouperLogo = new ToolStripMenuItem();
            CopierLogo = new ToolStripMenuItem();
            CollerLogo = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            MainMenu.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // MainMenu
            // 
            MainMenu.Items.AddRange(new ToolStripItem[] { btnFile, btnEdition, btnView, btnInfos, btnIP });
            MainMenu.Location = new Point(0, 0);
            MainMenu.Name = "MainMenu";
            MainMenu.Size = new Size(1012, 24);
            MainMenu.TabIndex = 0;
            MainMenu.Text = "menuStrip1";
            // 
            // btnFile
            // 
            btnFile.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem1, openToolStripMenuItem1, saveToolStripMenuItem1, saveAsToolStripMenuItem1 });
            btnFile.Name = "btnFile";
            btnFile.Size = new Size(37, 20);
            btnFile.Text = "File";
            // 
            // newToolStripMenuItem1
            // 
            newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            newToolStripMenuItem1.Size = new Size(114, 22);
            newToolStripMenuItem1.Text = "New";
            // 
            // openToolStripMenuItem1
            // 
            openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            openToolStripMenuItem1.Size = new Size(114, 22);
            openToolStripMenuItem1.Text = "Open";
            // 
            // saveToolStripMenuItem1
            // 
            saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            saveToolStripMenuItem1.Size = new Size(114, 22);
            saveToolStripMenuItem1.Text = "Save";
            // 
            // saveAsToolStripMenuItem1
            // 
            saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            saveAsToolStripMenuItem1.Size = new Size(114, 22);
            saveAsToolStripMenuItem1.Text = "Save As";
            saveAsToolStripMenuItem1.Click += saveAsToolStripMenuItem_Click;
            // 
            // btnEdition
            // 
            btnEdition.Name = "btnEdition";
            btnEdition.Size = new Size(56, 20);
            btnEdition.Text = "Edition";
            // 
            // btnView
            // 
            btnView.Name = "btnView";
            btnView.Size = new Size(44, 20);
            btnView.Text = "View";
            // 
            // btnInfos
            // 
            btnInfos.DropDownItems.AddRange(new ToolStripItem[] { controlCommentToolStripMenuItem, aboutToolStripMenuItem });
            btnInfos.Name = "btnInfos";
            btnInfos.Size = new Size(45, 20);
            btnInfos.Text = "Infos";
            // 
            // controlCommentToolStripMenuItem
            // 
            controlCommentToolStripMenuItem.Name = "controlCommentToolStripMenuItem";
            controlCommentToolStripMenuItem.Size = new Size(169, 22);
            controlCommentToolStripMenuItem.Text = "Control comment";
            controlCommentToolStripMenuItem.Click += controlCommentToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(169, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // btnIP
            // 
            btnIP.Enabled = false;
            btnIP.Name = "btnIP";
            btnIP.Size = new Size(12, 20);
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(127, 22);
            newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(127, 22);
            openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(127, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(32, 19);
            // 
            // newFormeToolStripMenuItem
            // 
            newFormeToolStripMenuItem.Name = "newFormeToolStripMenuItem";
            newFormeToolStripMenuItem.Size = new Size(127, 22);
            newFormeToolStripMenuItem.Text = "New Page";
            newFormeToolStripMenuItem.Click += newFormeToolStripMenuItem_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog";
            // 
            // pnlViewHost
            // 
            pnlViewHost.BackColor = SystemColors.ActiveBorder;
            pnlViewHost.Location = new Point(0, 57);
            pnlViewHost.Name = "pnlViewHost";
            pnlViewHost.Size = new Size(729, 459);
            pnlViewHost.TabIndex = 1;
            // 
            // lstToolbox
            // 
            lstToolbox.FormattingEnabled = true;
            lstToolbox.ItemHeight = 15;
            lstToolbox.Location = new Point(735, 57);
            lstToolbox.Name = "lstToolbox";
            lstToolbox.Size = new Size(277, 229);
            lstToolbox.TabIndex = 2;
            lstToolbox.SelectedIndexChanged += lstToolbox_SelectedIndexChanged;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Location = new Point(735, 292);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(277, 224);
            propertyGrid1.TabIndex = 3;
            propertyGrid1.PropertyValueChanged += propertyGrid1_PropertyValueChanged;
            // 
            // Nouveau
            // 
            Nouveau.BackgroundImage = (Image)resources.GetObject("Nouveau.BackgroundImage");
            Nouveau.BackgroundImageLayout = ImageLayout.Zoom;
            Nouveau.Name = "Nouveau";
            Nouveau.Size = new Size(12, 20);
            Nouveau.Tag = "New";
            Nouveau.Click += NouveauLogo_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.BackgroundImageLayout = ImageLayout.Zoom;
            menuStrip1.GripStyle = ToolStripGripStyle.Visible;
            menuStrip1.Items.AddRange(new ToolStripItem[] { Nouveau, Open, Save, SaveALL, Close, CloseALL, CouperLogo, CopierLogo, CollerLogo, toolStripMenuItem1, toolStripMenuItem2 });
            menuStrip1.Location = new Point(0, 24);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1012, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // Open
            // 
            Open.BackgroundImage = (Image)resources.GetObject("Open.BackgroundImage");
            Open.BackgroundImageLayout = ImageLayout.Zoom;
            Open.Name = "Open";
            Open.Size = new Size(12, 20);
            Open.Tag = "Open";
            Open.Click += OpenLogo_Click;
            // 
            // Save
            // 
            Save.BackgroundImage = (Image)resources.GetObject("Save.BackgroundImage");
            Save.BackgroundImageLayout = ImageLayout.Zoom;
            Save.Name = "Save";
            Save.Size = new Size(12, 20);
            Save.Tag = "Save";
            Save.Click += SavecLogoClick;
            // 
            // SaveALL
            // 
            SaveALL.BackgroundImage = (Image)resources.GetObject("SaveALL.BackgroundImage");
            SaveALL.BackgroundImageLayout = ImageLayout.Zoom;
            SaveALL.Name = "SaveALL";
            SaveALL.ShowShortcutKeys = false;
            SaveALL.Size = new Size(12, 20);
            SaveALL.Tag = "Save all";
            SaveALL.Click += SaveALL_Click;
            // 
            // Close
            // 
            Close.BackgroundImage = (Image)resources.GetObject("Close.BackgroundImage");
            Close.BackgroundImageLayout = ImageLayout.Zoom;
            Close.Name = "Close";
            Close.Size = new Size(12, 20);
            Close.Tag = "Close";
            Close.Click += Close_Click;
            // 
            // CloseALL
            // 
            CloseALL.BackgroundImage = (Image)resources.GetObject("CloseALL.BackgroundImage");
            CloseALL.BackgroundImageLayout = ImageLayout.Zoom;
            CloseALL.Name = "CloseALL";
            CloseALL.Size = new Size(12, 20);
            CloseALL.Tag = "Close all";
            CloseALL.Click += CloseALL_Click;
            // 
            // CouperLogo
            // 
            CouperLogo.BackgroundImage = (Image)resources.GetObject("CouperLogo.BackgroundImage");
            CouperLogo.BackgroundImageLayout = ImageLayout.Zoom;
            CouperLogo.Name = "CouperLogo";
            CouperLogo.Size = new Size(12, 20);
            CouperLogo.Tag = "Cut";
            CouperLogo.Click += CouperLogo_Click;
            // 
            // CopierLogo
            // 
            CopierLogo.BackgroundImage = (Image)resources.GetObject("CopierLogo.BackgroundImage");
            CopierLogo.BackgroundImageLayout = ImageLayout.Zoom;
            CopierLogo.Name = "CopierLogo";
            CopierLogo.Size = new Size(12, 20);
            CopierLogo.Tag = "Copy";
            CopierLogo.Click += CopierLogo_Click;
            // 
            // CollerLogo
            // 
            CollerLogo.BackgroundImage = (Image)resources.GetObject("CollerLogo.BackgroundImage");
            CollerLogo.BackgroundImageLayout = ImageLayout.Zoom;
            CollerLogo.Name = "CollerLogo";
            CollerLogo.Size = new Size(12, 20);
            CollerLogo.Tag = "Paste";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.BackgroundImage = (Image)resources.GetObject("toolStripMenuItem1.BackgroundImage");
            toolStripMenuItem1.BackgroundImageLayout = ImageLayout.Stretch;
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(12, 20);
            toolStripMenuItem1.Tag = "Deconnexion";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.BackgroundImage = (Image)resources.GetObject("toolStripMenuItem2.BackgroundImage");
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(12, 20);
            toolStripMenuItem2.Visible = false;
            // 
            // Forme1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(1012, 528);
            Controls.Add(menuStrip1);
            Controls.Add(propertyGrid1);
            Controls.Add(lstToolbox);
            Controls.Add(pnlViewHost);
            Controls.Add(MainMenu);
            MainMenuStrip = MainMenu;
            Name = "Forme1";
            Text = "Form1";
            FormClosing += FormePrincipaleClose;
            Load += Form1_Load;
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip MainMenu;
        private ToolStripMenuItem btnFile;
        private ToolStripMenuItem btnEdition;
        private ToolStripMenuItem btnView;
        private ToolStripMenuItem btnInfos;
        private ToolStripMenuItem btnIP;
        private OpenFileDialog openFileDialog;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private SaveFileDialog saveFileDialog1;
        private ToolStripMenuItem controlCommentToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Button button1;
        private Panel pnlViewHost;
        private ListBox lstToolbox;
        private PropertyGrid propertyGrid1;
        private ToolStripMenuItem newFormeToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem Nouveau;
        private ToolStripMenuItem Open;
        private ToolStripMenuItem Save;
        private ToolStripMenuItem SaveALL;
        private ToolStripMenuItem Close;
        private ToolStripMenuItem CloseALL;
        private ToolStripMenuItem CouperLogo;
        private ToolStripMenuItem CopierLogo;
        private ToolStripMenuItem CollerLogo;
        private ToolStripMenuItem newToolStripMenuItem1;
        private ToolStripMenuItem openToolStripMenuItem1;
        private ToolStripMenuItem saveToolStripMenuItem1;
        private ToolStripMenuItem saveAsToolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
    }
}
