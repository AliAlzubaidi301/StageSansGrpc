namespace StageCode.LIB
{
    partial class OrthoImage
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
            PictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            SuspendLayout();
            // 
            // PictureBox1
            // 
            PictureBox1.BackColor = Color.White;
            PictureBox1.Location = new Point(0, 0);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new Size(137, 103);
            PictureBox1.TabIndex = 0;
            PictureBox1.TabStop = false;
            PictureBox1.Click += PictureBox1_Click_1;
            // 
            // OrthoImage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(PictureBox1);
            Name = "OrthoImage";
            Size = new Size(137, 103);
            Load += OrthoImage_Load;
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public PictureBox PictureBox1;
    }
}