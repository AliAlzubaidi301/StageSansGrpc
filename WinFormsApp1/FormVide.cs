using StageCode;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OrthoDesigner
{
    public partial class FormVide : Form
    {
        private bool _isDragging = false;
        private bool _isResizing = false;
        private Point _dragStartPoint;
        private Size _resizeStartSize;
        private Point _resizeStartPoint;
        public Label label;
        private Point MousePosition = new Point();

        FormWindowsResize autreForme ;
        Panel pnlViewHost;

        public FormVide(Panel panelResize)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Enlever la bordureZ

            this.pnlViewHost = panelResize;
        }

        private void FormVide_Load(object sender, EventArgs e)
        {
            this.ClientSizeChanged += FormVide_ClientSizeChanged;
            this.MouseEnter += FormVide_MouseEnter;
            this.panel1.MouseEnter += FormVide_MouseEnter;

            this.DoubleBuffered = true;

            // Ajouter les boutons de contrôle
            AjouterBoutonsControle();

            // Ajouter l'événement pour déplacer la fenêtre
            panel2.MouseDown += FormVide_MouseDown;
            panel2.MouseMove += FormVide_MouseMove;
            panel2.MouseUp += FormVide_MouseUp;

            label = new Label();
            label.Text = this.Text;
            label.ForeColor = Color.White;

            label.Location = new Point(5,(panel2.Height -  label.Height) / 2 +2);

            this.TextChanged += FormVide_TextChanged;

            panel2.Controls.Add(label);

            // Ajouter l'événement pour le redimensionnement de la fenêtre
            panel1.MouseDown += FormVide_ResizeMouseDown;
            panel1.MouseMove += FormVide_ResizeMouseMove;
            panel1.MouseUp += FormVide_ResizeMouseUp;

            panel1.BackColor = Color.White; // Couleur de fond du panel
        }

        private void FormVide_TextChanged(object? sender, EventArgs e)
        {
            label.Text = this.Text;
        }

        private void FormVide_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _dragStartPoint = e.Location;
            }
        }

        private void FormVide_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                this.Location = new Point(
                    this.Left + e.X - _dragStartPoint.X,
                    this.Top + e.Y - _dragStartPoint.Y
                );
            }
        }

        private void FormVide_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void FormVide_ResizeMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isResizing = true;
                _resizeStartPoint = e.Location;
                _resizeStartSize = this.Size;
            }
        }

        private void FormVide_ResizeMouseMove(object sender, MouseEventArgs e)
        {
            this.Enabled = true;
            if (_isResizing)
            {
                int deltaWidth = e.X - _resizeStartPoint.X;
                int deltaHeight = e.Y - _resizeStartPoint.Y;
                this.Size = new Size(_resizeStartSize.Width + deltaWidth, _resizeStartSize.Height + deltaHeight);

                Invalidate();
            }
            else
            {
                const int borderSize = 10; // Zone de détection des bords

                bool left = e.X <= borderSize;
                bool right = e.X >= this.Width - borderSize;
                bool top = e.Y <= borderSize;
                bool bottom = e.Y >= panel1.Height - borderSize;

                if ((left && top) || (right && bottom))
                    this.Cursor = Cursors.SizeNWSE; // Coin haut-gauche ou bas-droit
                else if ((right && top) || (left && bottom))
                    this.Cursor = Cursors.SizeNESW; // Coin haut-droit ou bas-gauche
                else if (left || right)
                    this.Cursor = Cursors.SizeWE; // Côté gauche ou droit
                else if (top || bottom)
                    this.Cursor = Cursors.SizeNS; // Côté haut ou bas
                else
                    this.Cursor = Cursors.Default; // Curseur par défaut
            }

        }

        private void FormVide_ResizeMouseUp(object sender, MouseEventArgs e)
        {
            _isResizing = false;
        }

        private void AjouterBoutonsControle()
        {
            panel2.BackColor = Color.FromArgb(30, 30, 30); // Fond du panneau
            panel2.Dock = DockStyle.Top;
            panel2.Height = 40;

            Button btnFermer = new Button
            {
                Text = "✖",
                ForeColor = Color.White,
                BackColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 30),
                Location = new Point(panel2.Width - 45, 5) // Aligné à droite
            };
            btnFermer.Click += (s, e) => { this.Close(); };

            Button btnAgrandir = new Button
            {
                Text = "⬜",
                ForeColor = Color.White,
                BackColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 30),
                Location = new Point(panel2.Width - 90, 5)
            };

            Form autreForme = null;

            btnAgrandir.Click += (s, e) =>
            {
                this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                btnAgrandir.Text = this.WindowState == FormWindowState.Maximized ? "❐" : "⬜";
            };

            btnAgrandir.MouseEnter += (s, e) =>
            {
                if (autreForme == null || autreForme.IsDisposed)
                {
                    autreForme = new FormWindowsResize(this,pnlViewHost)
                    {
                        Size = new Size(132, 170), // Petite taille
                        StartPosition = FormStartPosition.Manual,
                        BackColor = Color.FromArgb(64, 64, 64),
                        FormBorderStyle = FormBorderStyle.None
                    };

                    // Ne pas cacher la fenêtre dès que la souris quitte
                    autreForme.MouseLeave += (sender, args) =>
                    {
                        // Si la souris quitte la fenêtre ou ses contrôles, cacher la fenêtre
                        if (!autreForme.Bounds.Contains(Cursor.Position))
                        {
                            bool isCursorOverControl = false;

                            // Vérifie si la souris est au-dessus de l'un des contrôles
                            foreach (Control control in autreForme.Controls)
                            {
                                if (control.Bounds.Contains(autreForme.PointToClient(Cursor.Position)))
                                {
                                    isCursorOverControl = true;
                                    break;
                                }
                            }

                            if (!isCursorOverControl)
                            {
                                autreForme.Hide();
                            }
                        }
                    };
                }

                // Positionner sous le bouton
                Point screenPosition = btnAgrandir.PointToScreen(new Point(btnAgrandir.Width / 2 - autreForme.Width / 3, btnAgrandir.Height));
                autreForme.Location = screenPosition;

                autreForme.Show();
            };

            btnAgrandir.MouseLeave += (s, e) =>
            {
                // Ne rien faire ici, on ne cache pas la fenêtre encore
            };

            Button btnReduire = new Button
            {
                Text = "➖",
                ForeColor = Color.White,
                BackColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 30),
                Location = new Point(panel2.Width - 135, 5)
            };
            btnReduire.Click += (s, e) =>
            {
                this.WindowState = FormWindowState.Minimized;
            };

            panel2.Controls.Add(btnFermer);
            panel2.Controls.Add(btnAgrandir);
            panel2.Controls.Add(btnReduire);

            panel2.Resize += (s, e) =>
            {
                btnFermer.Left = panel2.Width - 45;
                btnAgrandir.Left = panel2.Width - 90;
                btnReduire.Left = panel2.Width - 135;
            };
        }

        private void FormVide_MouseEnter(object sender, EventArgs e)
        {
            Forme1.forme = this;
            this.BringToFront();
        }

        private void FormVide_ClientSizeChanged(object sender, EventArgs e)
        {
            this.panel1.Size = this.ClientSize;
        }

        private void FormVide_Click(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}
