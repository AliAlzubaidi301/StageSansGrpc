namespace OrthoDesigner
{
    partial class FormWindowsResize
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
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Location = new Point(16, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(46, 55);
            panel1.TabIndex = 6;
            // 
            // panel2
            // 
            panel2.Location = new Point(74, 12);
            panel2.Name = "panel2";
            panel2.Size = new Size(46, 55);
            panel2.TabIndex = 7;
            // 
            // panel3
            // 
            panel3.Location = new Point(16, 83);
            panel3.Name = "panel3";
            panel3.Size = new Size(46, 32);
            panel3.TabIndex = 7;
            // 
            // panel4
            // 
            panel4.Location = new Point(74, 83);
            panel4.Name = "panel4";
            panel4.Size = new Size(46, 32);
            panel4.TabIndex = 8;
            // 
            // panel5
            // 
            panel5.Location = new Point(16, 121);
            panel5.Name = "panel5";
            panel5.Size = new Size(46, 32);
            panel5.TabIndex = 8;
            // 
            // panel6
            // 
            panel6.Location = new Point(74, 121);
            panel6.Name = "panel6";
            panel6.Size = new Size(46, 32);
            panel6.TabIndex = 8;
            // 
            // FormWindowsResize
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(132, 165);
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormWindowsResize";
            Text = "FormWindowsResize";
            Load += FormWindowsResize_Load;
            ResumeLayout(false);
        }

        #endregion
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
    }
}