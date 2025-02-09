namespace StageCode
{
    partial class Form1
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
            MainMenu = new MenuStrip();
            btnFile = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            newFormeToolStripMenuItem = new ToolStripMenuItem();
            btnEdition = new ToolStripMenuItem();
            btnView = new ToolStripMenuItem();
            btnInfos = new ToolStripMenuItem();
            controlCommentToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            btnVersion = new ToolStripMenuItem();
            openFileDialog = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            pnlViewHost = new Panel();
            lstToolbox = new ListBox();
            propertyGrid1 = new PropertyGrid();
            MainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // MainMenu
            // 
            MainMenu.Items.AddRange(new ToolStripItem[] { btnFile, btnEdition, btnView, btnInfos, btnVersion });
            MainMenu.Location = new Point(0, 0);
            MainMenu.Name = "MainMenu";
            MainMenu.Size = new Size(1012, 24);
            MainMenu.TabIndex = 0;
            MainMenu.Text = "menuStrip1";
            // 
            // btnFile
            // 
            btnFile.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, newFormeToolStripMenuItem });
            btnFile.Name = "btnFile";
            btnFile.Size = new Size(37, 20);
            btnFile.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(135, 22);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(135, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(135, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(135, 22);
            saveAsToolStripMenuItem.Text = "Save As";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // newFormeToolStripMenuItem
            // 
            newFormeToolStripMenuItem.Name = "newFormeToolStripMenuItem";
            newFormeToolStripMenuItem.Size = new Size(135, 22);
            newFormeToolStripMenuItem.Text = "New Forme";
            newFormeToolStripMenuItem.Click += newFormeToolStripMenuItem_Click;
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
            // btnVersion
            // 
            btnVersion.Enabled = false;
            btnVersion.Name = "btnVersion";
            btnVersion.Size = new Size(29, 20);
            btnVersion.Text = "V ";
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog";
            // 
            // pnlViewHost
            // 
            pnlViewHost.BackColor = Color.White;
            pnlViewHost.Location = new Point(0, 31);
            pnlViewHost.Name = "pnlViewHost";
            pnlViewHost.Size = new Size(729, 485);
            pnlViewHost.TabIndex = 1;
            // 
            // lstToolbox
            // 
            lstToolbox.FormattingEnabled = true;
            lstToolbox.ItemHeight = 15;
            lstToolbox.Location = new Point(735, 27);
            lstToolbox.Name = "lstToolbox";
            lstToolbox.Size = new Size(277, 259);
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1012, 528);
            Controls.Add(propertyGrid1);
            Controls.Add(lstToolbox);
            Controls.Add(pnlViewHost);
            Controls.Add(MainMenu);
            MainMenuStrip = MainMenu;
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip MainMenu;
        private ToolStripMenuItem btnFile;
        private ToolStripMenuItem btnEdition;
        private ToolStripMenuItem btnView;
        private ToolStripMenuItem btnInfos;
        private ToolStripMenuItem btnVersion;
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
    }
}
