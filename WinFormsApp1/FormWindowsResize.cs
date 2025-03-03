using System;
using System.Drawing;
using System.Windows.Forms;

namespace OrthoDesigner
{
    public partial class FormWindowsResize : Form
    {
        FormVide forme;
        Panel panel;
        System.Windows.Forms.Timer animationTimer;
        int targetX, targetY, targetWidth, targetHeight;
        int stepSize = 10;

        public FormWindowsResize(FormVide forme, Panel panel)
        {
            InitializeComponent();
            this.forme = forme;
            this.panel = panel;
            this.animationTimer = new System.Windows.Forms.Timer();
            this.animationTimer.Interval = 1;
            this.animationTimer.Tick += AnimationTimer_Tick;
            this.stepSize = 10;

            this.BackColor = Color.FromArgb(30, 30, 30);
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            this.Hide();

            int tolerance = 5;

            if (Math.Abs(forme.Location.X - targetX) > tolerance)
                forme.Location = new Point(forme.Location.X + (forme.Location.X < targetX ? stepSize : -stepSize), forme.Location.Y);

            if (Math.Abs(forme.Location.Y - targetY) > tolerance)
                forme.Location = new Point(forme.Location.X, forme.Location.Y + (forme.Location.Y < targetY ? stepSize : -stepSize));

            if (Math.Abs(forme.Width - targetWidth) > tolerance)
                forme.Width += (forme.Width < targetWidth ? stepSize : -stepSize);

            if (Math.Abs(forme.Height - targetHeight) > tolerance)
                forme.Height += (forme.Height < targetHeight ? stepSize : -stepSize);

            if (Math.Abs(forme.Location.X - targetX) <= tolerance &&
                Math.Abs(forme.Location.Y - targetY) <= tolerance &&
                Math.Abs(forme.Width - targetWidth) <= tolerance &&
                Math.Abs(forme.Height - targetHeight) <= tolerance)
            {
                animationTimer.Stop();
                this.Hide();
                this.Dispose();
            }
        }

        private void FormWindowsResize_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;

            foreach (Control c in this.Controls)
            {
                if (c is Panel pic)
                {
                    pic.MouseEnter += C_MouseEnter;
                    pic.MouseLeave += C_MouseLeave;
                    pic.Click += Pic_Click;
                    pic.Tag = this.Controls.GetChildIndex(pic) + 1;

                    pic.BackColor = Color.FromArgb(60, 60, 60);
                    pic.BorderStyle = BorderStyle.None;
                    //pic.Padding = new Padding(10);
                    //pic.Margin = new Padding(8);
                    //pic.Width = 100;
                    //pic.Height = 100;
                    pic.Cursor = Cursors.Hand;
                    pic.Paint += Pic_Paint;
                }
            }
        }

        private void Pic_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel pic)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.White, 2))
                {
                    e.Graphics.DrawRectangle(pen, 10, 10, pic.Width - 20, pic.Height - 20);
                }

                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, new Rectangle(12, 12, pic.Width - 24, pic.Height - 24));
                }

                using (SolidBrush gradientBrush = new SolidBrush(Color.FromArgb(255, 45, 52, 54)))
                {
                    e.Graphics.FillRectangle(gradientBrush, new Rectangle(10, 10, pic.Width - 20, pic.Height - 20));
                }
            }
        }

        private void Pic_Click(object sender, EventArgs e)
        {
            if (sender is Panel pic)
            {
                int index = Convert.ToInt32(pic.Tag);
                int halfWidth = panel.Width / 2;
                int halfHeight = panel.Height / 2;

                (targetX, targetY, targetWidth, targetHeight) = index switch
                {
                    1 => (halfWidth, halfHeight, halfWidth, halfHeight),
                    2 => (0, halfHeight, halfWidth, halfHeight),
                    3 => (halfWidth, 0, halfWidth, halfHeight),
                    4 => (0, 0, halfWidth, halfHeight),
                    5 => (panel.Width - halfWidth, 0, halfWidth, panel.Height),
                    6 => (0, 0, halfWidth, panel.Height),
                    _ => (0, 0, panel.Width, panel.Height)
                };

                animationTimer.Start();
            }
        }

        private void C_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Panel pic)
            {
                pic.BackColor = Color.FromArgb(100, 100, 100);
                pic.Invalidate();
            }
        }

        private void C_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Panel pic)
            {
                pic.BackColor = Color.FromArgb(60, 60, 60);
                pic.Invalidate();
            }
        }
    }
}
