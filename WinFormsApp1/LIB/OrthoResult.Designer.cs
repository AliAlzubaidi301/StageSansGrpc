namespace StageCode.LIB
{
    partial class OrthoResult
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
            TextBox1 = new TextBox();
            SuspendLayout();
            // 
            // TextBox1
            // 
            TextBox1.Enabled = false;
            TextBox1.Location = new Point(3, 3);
            TextBox1.Name = "TextBox1";
            TextBox1.Size = new Size(100, 23);
            TextBox1.TabIndex = 0;
            // 
            // OrthoResult
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TextBox1);
            Name = "OrthoResult";
            Size = new Size(106, 36);
            Load += OrthoResult_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TextBox1;
    }
}