namespace StageCode.LIB
{
    partial class OrthoCombo
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
            ComboBox1 = new ComboBox();
            SuspendLayout();
            // 
            // ComboBox1
            // 
            ComboBox1.Enabled = false;
            ComboBox1.FormattingEnabled = true;
            ComboBox1.Location = new Point(3, 3);
            ComboBox1.Name = "ComboBox1";
            ComboBox1.Size = new Size(121, 23);
            ComboBox1.TabIndex = 0;
            ComboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged_1;
            // 
            // OrthoCombo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ComboBox1);
            Name = "OrthoCombo";
            Size = new Size(126, 39);
            Load += OrthoCombo_Load;
            ResumeLayout(false);
        }

        #endregion

        private ComboBox ComboBox1;
    }
}