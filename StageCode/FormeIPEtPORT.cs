using System;
using System.Drawing;
using System.Windows.Forms;

namespace OrthoDesigner
{
    public partial class FormeIPEtPORT : Form
    {
        // La Libraire c'est OrthoDyne quelques Choses

        private TextBox txtIP;
        private TextBox txtPort;
        private Button btnSubmit;
        private Label lblIP;
        private Label lblPort;
        private Label lblTitle;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private float speed = 0;
        private float acceleration = 0.3f;
        private float maxSpeed = 10;
        private float damping = 0.50f;

        public FormeIPEtPORT()
        {
            InitializeComponent();
            InitializeCustomComponents();

            this.MouseEnter += (s, e) => timer.Stop();
            this.Opacity = 0; // Démarrage avec une transition de fondu
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.Size = new Size(450, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Connexion réseau";
            this.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            lblTitle = new Label()
            {
                Text = "Veuillez entrer les informations de connexion",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 20),
                Size = new Size(350, 40)
            };

            lblIP = new Label()
            {
                Text = "Adresse IP :",
                ForeColor = Color.White,
                Location = new Point(100, 60),
                Size = new Size(250, 20)
            };

            txtIP = CreateStyledTextBox(100, 90, "Entrez l'adresse IP");

            lblPort = new Label()
            {
                Text = "Numéro de port :",
                ForeColor = Color.White,
                Location = new Point(100, 140),
                Size = new Size(250, 20)
            };

            txtPort = CreateStyledTextBox(100, 170, "Entrez le numéro de port");

            btnSubmit = new Button()
            {
                Text = "Se connecter",
                Location = new Point(100, 230),
                Size = new Size(250, 45),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 122, 204),
                FlatStyle = FlatStyle.Flat
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.Click += BtnSubmit_Click;
            btnSubmit.MouseEnter += (s, e) => { btnSubmit.BackColor = Color.FromArgb(0, 102, 180); };
            btnSubmit.MouseLeave += (s, e) => { btnSubmit.BackColor = Color.FromArgb(0, 122, 204); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblIP);
            this.Controls.Add(txtIP);
            this.Controls.Add(lblPort);
            this.Controls.Add(txtPort);
            this.Controls.Add(btnSubmit);
        }

        private TextBox CreateStyledTextBox(int x, int y, string placeholder)
        {
            return new TextBox()
            {
                Location = new Point(x, y),
                Size = new Size(250, 35),
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 50),
                BorderStyle = BorderStyle.None,
                PlaceholderText = placeholder,
                TextAlign = HorizontalAlignment.Center
            };
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string ip = txtIP.Text;
            string port = txtPort.Text;

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
            {
                MessageBox.Show("Veuillez entrer une IP et un port valides.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (int.TryParse(port, out int portNumber) && portNumber > 0 && portNumber <= 65535)
            {
                MessageBox.Show($"IP : {ip}\nPort : {portNumber}", "Informations Soumises", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Le port doit être un nombre entre 1 et 65535.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormeIPEtPORT_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            Location = new Point(this.Location.X, -this.Height);
            timer.Tick += Timer_Tick;
            timer.Interval = 15;
            timer.Start();
            FadeIn();
        }

        private async void FadeIn()
        {
            for (double i = 0; i <= 1; i += 0.05)
            {
                this.Opacity = i;
                await System.Threading.Tasks.Task.Delay(15);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            speed = Math.Min(speed + acceleration, maxSpeed);
            this.Location = new Point(this.Location.X, (int)(this.Location.Y + speed));
            int maxY = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            if (this.Location.Y >= maxY)
            {
                speed = -speed * damping;
                if (Math.Abs(speed) < 1)
                {
                    timer.Stop();
                    this.Location = new Point(this.Location.X, maxY);
                }
                else if (Math.Abs(speed) < 5)
                {
                    // speed = -speed * reboundDamping;
                }
            }
        }
    }
}