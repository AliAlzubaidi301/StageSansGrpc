using Orthodyne.CoreCommunicationLayer.Controllers;
using StageCode;
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;

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
        public static string ip = "";
        public string portNumbers = "";

        // Variable de langue
        public static int Langue = 1; // 1 = English, 2 = Chinese, 3 = German, 4 = French, 5 = Lithuanian

        public FormeIPEtPORT()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void FormeIPEtPORT_Load(object sender, EventArgs e)
        {
            Langue = Forme1.Langue;
            SetLanguage(); // Met à jour les textes en fonction de la langue
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
                txtIP[i] = CreateStyledTextBox(100 + (i * 70), 90, "0");
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
            var a = new TextBox()
            {
                Location = new Point(x, y),
                Size = new Size(60, 35),
                Font = new System.Drawing.Font("Segoe UI", 14),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 50),
                BorderStyle = BorderStyle.None,
                PlaceholderText = placeholder,
                TextAlign = HorizontalAlignment.Center
            };

            if (a.PlaceholderText == "Entrez le numéro de port")
            {
                a.Width = 270;
            }

            return a;
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
            string IP = string.Join(".", Array.ConvertAll(txtIP, txt => txt.Text));
            ip = IP;
            string port = txtPort.Text;

            if (string.IsNullOrEmpty(port) || !int.TryParse(port, out int portNumber) || portNumber <= 0 || portNumber > 65535)
            {
                MessageBox.Show(GetMessage("ErreurPort"), GetMessage("Erreur"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(IP, 1000);
                    if (reply.Status == IPStatus.Success)
                    {
                        connecter = true;
                        this.portNumbers = portNumber.ToString();

                        this.Close();
                    }
                    else
                    {
                        connecter = false;
                        MessageBox.Show(string.Format(GetMessage("ErreurPing"), IP), GetMessage("Erreur"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(GetMessage("ErreurPingException"), ex.Message), GetMessage("Erreur"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetLanguage()
        {
            switch (Langue)
            {
                case 1: // English
                    lblTitle.Text = "Please enter connection information";
                    lblIP.Text = "IP Address:";
                    lblPort.Text = "Port Number:";
                    btnSubmit.Text = "Connect";
                    break;
                case 2: // Chinese
                    lblTitle.Text = "请输入连接信息";
                    lblIP.Text = "IP 地址：";
                    lblPort.Text = "端口号：";
                    btnSubmit.Text = "连接";
                    break;
                case 3: // German
                    lblTitle.Text = "Bitte geben Sie die Verbindungsinformationen ein";
                    lblIP.Text = "IP-Adresse:";
                    lblPort.Text = "Portnummer:";
                    btnSubmit.Text = "Verbinden";
                    break;
                case 4: // French
                    lblTitle.Text = "Veuillez entrer les informations de connexion";
                    lblIP.Text = "Adresse IP :";
                    lblPort.Text = "Numéro de port :";
                    btnSubmit.Text = "Se connecter";
                    break;
                case 5: // Lithuanian
                    lblTitle.Text = "Įveskite prisijungimo informaciją";
                    lblIP.Text = "IP adresas:";
                    lblPort.Text = "Prievado numeris:";
                    btnSubmit.Text = "Prisijungti";
                    break;
                default:
                    lblTitle.Text = "Veuillez entrer les informations de connexion";
                    lblIP.Text = "Adresse IP :";
                    lblPort.Text = "Numéro de port :";
                    btnSubmit.Text = "Se connecter";
                    break;
            }
        }

        // Méthode pour obtenir le texte en fonction de la langue
        private string GetMessage(string key)
        {
            switch (Langue)
            {
                case 1: // English
                    switch (key)
                    {
                        case "Erreur": return "Error";
                        case "ErreurPort": return "Please enter a valid IP and port between 1 and 65535.";
                        case "ErreurPing": return "Unable to reach IP address: {0}.";
                        case "ErreurPingException": return "Error during ping: {0}";
                        default: return "";
                    }
                case 2: // Chinese
                    switch (key)
                    {
                        case "Erreur": return "错误";
                        case "ErreurPort": return "请输入有效的IP和端口号，范围在1到65535之间。";
                        case "ErreurPing": return "无法访问IP地址：{0}。";
                        case "ErreurPingException": return "ping 错误：{0}";
                        default: return "";
                    }
                case 3: // German
                    switch (key)
                    {
                        case "Erreur": return "Fehler";
                        case "ErreurPort": return "Bitte geben Sie eine gültige IP und einen Port zwischen 1 und 65535 ein.";
                        case "ErreurPing": return "IP-Adresse kann nicht erreicht werden: {0}.";
                        case "ErreurPingException": return "Fehler beim Ping: {0}";
                        default: return "";
                    }
                case 4: // French
                    switch (key)
                    {
                        case "Erreur": return "Erreur";
                        case "ErreurPort": return "Veuillez entrer une IP valide et un port entre 1 et 65535.";
                        case "ErreurPing": return "Impossible de joindre l'adresse IP : {0}.";
                        case "ErreurPingException": return "Erreur lors du ping : {0}";
                        default: return "";
                    }
                case 5: // Lithuanian
                    switch (key)
                    {
                        case "Erreur": return "Klaida";
                        case "ErreurPort": return "Įveskite galiojantį IP ir prievado numerį nuo 1 iki 65535.";
                        case "ErreurPing": return "Nepavyko pasiekti IP adreso: {0}.";
                        case "ErreurPingException": return "Ping klaida: {0}";
                        default: return "";
                    }
                default:
                    return "";
            }
        }
    }
}
