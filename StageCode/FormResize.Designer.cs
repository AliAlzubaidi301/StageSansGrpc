namespace StageCode
{
    partial class FormResize
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
            groupBoxAvant = new GroupBox();
            label6 = new Label();
            label5 = new Label();
            label2 = new Label();
            label1 = new Label();
            groupBoxApres = new GroupBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            label4 = new Label();
            label3 = new Label();
            btnOK = new Button();
            btnResize = new Button();
            groupBoxAvant.SuspendLayout();
            groupBoxApres.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxAvant
            // 
            groupBoxAvant.Controls.Add(label6);
            groupBoxAvant.Controls.Add(label5);
            groupBoxAvant.Controls.Add(label2);
            groupBoxAvant.Controls.Add(label1);
            groupBoxAvant.Location = new Point(12, 12);
            groupBoxAvant.Name = "groupBoxAvant";
            groupBoxAvant.Size = new Size(201, 100);
            groupBoxAvant.TabIndex = 0;
            groupBoxAvant.TabStop = false;
            groupBoxAvant.Text = "groupBox1";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(141, 62);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 3;
            label6.Text = "label6";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(141, 25);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 2;
            label5.Text = "label5";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 62);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 1;
            label2.Text = "label2";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 25);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 0;
            label1.Text = "label1";
            // 
            // groupBoxApres
            // 
            groupBoxApres.Controls.Add(textBox2);
            groupBoxApres.Controls.Add(textBox1);
            groupBoxApres.Controls.Add(label4);
            groupBoxApres.Controls.Add(label3);
            groupBoxApres.Location = new Point(12, 118);
            groupBoxApres.Name = "groupBoxApres";
            groupBoxApres.Size = new Size(200, 100);
            groupBoxApres.TabIndex = 1;
            groupBoxApres.TabStop = false;
            groupBoxApres.Text = "groupBox2";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(141, 69);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(38, 23);
            textBox2.TabIndex = 5;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(141, 31);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(38, 23);
            textBox1.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 72);
            label4.Name = "label4";
            label4.Size = new Size(38, 15);
            label4.TabIndex = 3;
            label4.Text = "label4";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 39);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 2;
            label3.Text = "label3";
            // 
            // btnOK
            // 
            btnOK.Location = new Point(12, 238);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 2;
            btnOK.Text = "button1";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnResize
            // 
            btnResize.Location = new Point(137, 238);
            btnResize.Name = "btnResize";
            btnResize.Size = new Size(75, 23);
            btnResize.TabIndex = 3;
            btnResize.Text = "button2";
            btnResize.UseVisualStyleBackColor = true;
            btnResize.Click += btnResize_Click;
            // 
            // FormResize
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(225, 277);
            Controls.Add(btnResize);
            Controls.Add(btnOK);
            Controls.Add(groupBoxApres);
            Controls.Add(groupBoxAvant);
            Name = "FormResize";
            Text = "FormResize";
            Load += FormResize_Load;
            groupBoxAvant.ResumeLayout(false);
            groupBoxAvant.PerformLayout();
            groupBoxApres.ResumeLayout(false);
            groupBoxApres.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxAvant;
        private GroupBox groupBoxApres;
        private Label label2;
        private Label label1;
        private Label label4;
        private Label label3;
        private Label label6;
        private Label label5;
        private TextBox textBox2;
        private TextBox textBox1;
        private Button btnOK;
        private Button btnResize;
    }
}