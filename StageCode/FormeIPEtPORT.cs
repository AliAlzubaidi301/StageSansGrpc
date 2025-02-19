using System;
using System.Drawing;
using System.Windows.Forms;

namespace OrthoDesigner
{
    public partial class FormeIPEtPORT : Form
    {
        // Déclaration des contrôles TextBox, Label et Button
        private TextBox txtIP;
        private TextBox txtPort;
        private Button btnSubmit;
        private Label lblIP;
        private Label lblPort;
        private Label lblTitle;

        public FormeIPEtPORT()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Personnalisation du fond de la fenêtre
            this.BackColor = Color.FromArgb(34, 34, 34); // Fond sombre
            this.Size = new Size(450, 350); // Taille ajustée de la fenêtre
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Désactive la possibilité de redimensionner la fenêtre
            this.MaximizeBox = false; // Désactive le bouton de maximisation
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Connexion réseau"; // Titre de la fenêtre
            this.Font = new Font("Segoe UI", 12, FontStyle.Regular); // Police moderne

            // Titre du formulaire
            lblTitle = new Label();
            lblTitle.Text = "Veuillez entrer les informations de connexion";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Location = new Point(50, 20);
            lblTitle.Size = new Size(350, 40);

            // Création et personnalisation de la TextBox pour l'IP
            txtIP = new TextBox();
            txtIP.Location = new Point(100, 90);
            txtIP.Size = new Size(250, 35);
            txtIP.Font = new Font("Segoe UI", 14);
            txtIP.ForeColor = Color.White;
            txtIP.BackColor = Color.FromArgb(50, 50, 50); // Fond sombre
            txtIP.BorderStyle = BorderStyle.None;
            txtIP.Padding = new Padding(10);
            txtIP.PlaceholderText = "Entrez l'adresse IP";
            txtIP.TextAlign = HorizontalAlignment.Center;

            // Création et personnalisation du Label pour l'IP
            lblIP = new Label();
            lblIP.Text = "Adresse IP :";
            lblIP.ForeColor = Color.White;
            lblIP.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblIP.Location = new Point(100, 60);
            lblIP.Size = new Size(250, 20);

            // Création et personnalisation de la TextBox pour le Port
            txtPort = new TextBox();
            txtPort.Location = new Point(100, 150);
            txtPort.Size = new Size(250, 35);
            txtPort.Font = new Font("Segoe UI", 14);
            txtPort.ForeColor = Color.White;
            txtPort.BackColor = Color.FromArgb(50, 50, 50); // Fond sombre
            txtPort.BorderStyle = BorderStyle.None;
            txtPort.Padding = new Padding(10);
            txtPort.PlaceholderText = "Entrez le numéro de port";
            txtPort.TextAlign = HorizontalAlignment.Center;

            // Création et personnalisation du Label pour le Port
            lblPort = new Label();
            lblPort.Text = "Numéro de port :";
            lblPort.ForeColor = Color.White;
            lblPort.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblPort.Location = new Point(100, 120);
            lblPort.Size = new Size(250, 20);

            // Création et personnalisation du Button
            btnSubmit = new Button();
            btnSubmit.Text = "Se connecter";
            btnSubmit.Location = new Point(100, 210);
            btnSubmit.Size = new Size(250, 45);
            btnSubmit.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.BackColor = Color.FromArgb(0, 122, 204); // Bleu vif
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.Click += BtnSubmit_Click;
            btnSubmit.MouseEnter += (s, e) => { btnSubmit.BackColor = Color.FromArgb(0, 102, 180); };
            btnSubmit.MouseLeave += (s, e) => { btnSubmit.BackColor = Color.FromArgb(0, 122, 204); };

            // Ajouter les contrôles à la forme
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblIP);
            this.Controls.Add(txtIP);
            this.Controls.Add(lblPort);
            this.Controls.Add(txtPort);
            this.Controls.Add(btnSubmit);
        }

        // Action à effectuer lorsque le bouton "Se connecter" est cliqué
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string ip = txtIP.Text;
            string port = txtPort.Text;

            // Validation basique de l'IP et du port
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
            {
                MessageBox.Show("Veuillez entrer une IP et un port valides.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Essayer de convertir le port en un entier
                if (int.TryParse(port, out int portNumber) && portNumber > 0 && portNumber <= 65535)
                {
                    // Si tout est valide, afficher les informations
                    MessageBox.Show($"IP : {ip}\nPort : {portNumber}", "Informations Soumises", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Le port doit être un nombre entre 1 et 65535.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormeIPEtPORT_Load(object sender, EventArgs e)
        {

        }
    }
}
