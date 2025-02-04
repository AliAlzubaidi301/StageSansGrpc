namespace StageCode.LIB
{
    partial class OrthoPbar
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
            ProgressBar1 = new ProgressBar();
            Label1 = new Label();
            SuspendLayout();
            // 
            // ProgressBar1
            // 
            ProgressBar1.Location = new Point(3, 3);
            ProgressBar1.Name = "ProgressBar1";
            ProgressBar1.Size = new Size(100, 23);
            ProgressBar1.TabIndex = 0;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Location = new Point(30, 3);
            Label1.Name = "Label1";
            Label1.Size = new Size(38, 15);
            Label1.TabIndex = 1;
            Label1.Text = "label1";
            // 
            // OrthoPbar
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Label1);
            Controls.Add(ProgressBar1);
            Name = "OrthoPbar";
            Size = new Size(110, 40);
            Load += OrthoPbar_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar ProgressBar1;
        private Label Label1;
    }
}