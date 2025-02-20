using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace OrthoDesigner
{
    public partial class FormeIPEtPORT : Form
    {
        private TextBox[] txtIP = new TextBox[4];
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
        public static bool connecter = false;

        public FormeIPEtPORT()
        {
            InitializeComponent();
            InitializeCustomComponents();

            this.MouseEnter += (s, e) => timer.Stop();
            this.Opacity = 0;
        }

        private void FormeIPEtPORT_Load(object sender, EventArgs e)
        {
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
            }
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.Size = new Size(450, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Connexion réseau";
            this.Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Regular);

            lblTitle = new Label()
            {
                Text = "Veuillez entrer les informations de connexion",
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Segoe UI", 16, FontStyle.Bold),
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

            for (int i = 0; i < 4; i++)
            {
                txtIP[i] = CreateStyledTextBox(100 + (i * 60), 90, "0");
                txtIP[i].MaxLength = 3;
                txtIP[i].KeyPress += TxtIP_KeyPress;
                txtIP[i].TextChanged += TxtIP_TextChanged;
                this.Controls.Add(txtIP[i]);
            }

            lblPort = new Label()
            {
                Text = "Numéro de port :",
                ForeColor = Color.White,
                Location = new Point(100, 140),
                Size = new Size(250, 20)
            };

            txtPort = CreateStyledTextBox(100, 170, "Entrez le numéro de port");
            txtPort.KeyPress += TxtPort_KeyPress;

            btnSubmit = new Button()
            {
                Text = "Se connecter",
                Location = new Point(100, 230),
                Size = new Size(250, 45),
                Font = new System.Drawing.Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 122, 204),
                FlatStyle = FlatStyle.Flat
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.Click += BtnSubmit_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblIP);
            this.Controls.Add(lblPort);
            this.Controls.Add(txtPort);
            this.Controls.Add(btnSubmit);
        }

        private TextBox CreateStyledTextBox(int x, int y, string placeholder)
        {
            return new TextBox()
            {
                Location = new Point(x, y),
                Size = new Size(50, 35),
                Font = new System.Drawing.Font("Segoe UI", 14),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 50),
                BorderStyle = BorderStyle.None,
                PlaceholderText = placeholder,
                TextAlign = HorizontalAlignment.Center
            };
        }

        private void TxtIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtIP_TextChanged(object sender, EventArgs e)
        {
            TextBox? txt = sender as TextBox;

            if (string.IsNullOrEmpty(txt.Text))
            {
                return; 
            }

            int value;
            if (int.TryParse(txt.Text, out value))
            {
                if (value > 256)
                {
                    txt.Text = 255.ToString();
                    return;
                }
            }
            else
            {
                return;
            }

            if (txt.Text.Length == 3)
            {
                this.SelectNextControl(txt, true, true, true, true);
            }
        }


        private void TxtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            string ip = string.Join(".", Array.ConvertAll(txtIP, txt => txt.Text));
            string port = txtPort.Text;

            if (string.IsNullOrEmpty(port) || !int.TryParse(port, out int portNumber) || portNumber <= 0 || portNumber > 65535)
            {
                MessageBox.Show("Veuillez entrer une IP valide et un port entre 1 et 65535.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(ip, 1000);
                    if (reply.Status == IPStatus.Success)
                    {
                        MessageBox.Show($"Connexion à {ip}:{portNumber} réussie !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        connecter = true;
                        this.Close();
                    }
                    else
                    {
                        connecter = false;

                        MessageBox.Show($"Impossible de joindre l'adresse IP : {ip}.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du ping : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
