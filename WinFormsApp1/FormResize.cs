using System;
using System.Drawing;
using System.Windows.Forms;

namespace StageCode
{
    public partial class FormResize : Form
    {
        private Forme1 forme;

        public FormResize(Forme1 forme)
        {
            InitializeComponent();
            this.forme = forme;

            ConfigureAnchors();

            this.SizeChanged += FormResize_SizeChanged;

            ChangeLanguage();
        }

        private void ChangeLanguage()
        {
            switch (Forme1.Langue)
            {
                case 1: // English
                    this.groupBoxAvant.Text = "Old Size";
                    this.groupBoxApres.Text = "New Size";
                    this.label1.Text = "Width";
                    this.label2.Text = "Height";
                    label3.Text = "Width";
                    label4.Text = "Height";
                    this.btnOK.Text = "OK";
                    this.btnResize.Text = "Cancel";
                    break;

                case 2: // Chinese
                    this.groupBoxAvant.Text = "旧尺寸";
                    this.groupBoxApres.Text = "新尺寸";
                    this.label1.Text = "宽度";
                    this.label2.Text = "高度";
                    label3.Text = "宽度";
                    label4.Text = "高度";
                    this.btnOK.Text = "确定";
                    this.btnResize.Text = "取消";
                    break;

                case 3: // German
                    this.groupBoxAvant.Text = "Alte Größe";
                    this.groupBoxApres.Text = "Neue Größe";
                    this.label1.Text = "Breite";
                    this.label2.Text = "Höhe";
                    label3.Text = "Breite";
                    label4.Text = "Höhe";
                    this.btnOK.Text = "OK";
                    this.btnResize.Text = "Abbrechen";
                    break;

                case 4: // French
                    this.groupBoxAvant.Text = "Ancienne taille";
                    this.groupBoxApres.Text = "Nouvelle taille";
                    this.label1.Text = "Largeur";
                    this.label2.Text = "Hauteur";
                    label3.Text = "Largeur";
                    label4.Text = "Hauteur";
                    this.btnOK.Text = "OK";
                    this.btnResize.Text = "Annuler";
                    break;

                case 5: // Lithuanian
                    this.groupBoxAvant.Text = "Senas dydis";
                    this.groupBoxApres.Text = "Naujas dydis";
                    this.label1.Text = "Plotis";
                    this.label2.Text = "Aukštis";
                    label3.Text = "Plotis";
                    label4.Text = "Aukštis";
                    this.btnOK.Text = "Gerai";
                    this.btnResize.Text = "Atšaukti";
                    break;

                default:
                    this.groupBoxAvant.Text = "Old Size";
                    this.groupBoxApres.Text = "New Size";
                    this.label1.Text = "Width";
                    this.label2.Text = "Height";
                    label3.Text = "Width";
                    label4.Text = "Height";
                    this.btnOK.Text = "OK";
                    this.btnResize.Text = "Cancel";
                    break;
            }
        }

        private void ConfigureAnchors()
        {
            this.groupBoxAvant.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.groupBoxApres.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnResize.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.label2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            this.textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void FormResize_SizeChanged(object sender, EventArgs e)
        {
            int margin = 10;
            int groupBoxHeight = (int)(this.ClientSize.Height * 0.34);

            this.groupBoxAvant.Height = groupBoxHeight;
            this.groupBoxApres.Height = groupBoxHeight;
            this.groupBoxApres.Top = this.groupBoxAvant.Bottom + margin;

            int buttonSpacing = 10;
            int buttonWidth = this.btnOK.Width;

            this.btnOK.Top = this.ClientSize.Height - this.btnOK.Height - buttonSpacing;
            this.btnResize.Top = this.btnOK.Top;
            this.btnOK.Left = this.ClientSize.Width - buttonWidth * 2 - buttonSpacing;
            this.btnResize.Left = this.ClientSize.Width - buttonWidth - buttonSpacing;

            float fontSize = Math.Max(8, this.ClientSize.Width / 50);
            this.label1.Font = new Font(this.label1.Font.FontFamily, fontSize);
            this.label2.Font = new Font(this.label2.Font.FontFamily, fontSize);
            this.label5.Font = new Font(this.label5.Font.FontFamily, fontSize);
            this.label6.Font = new Font(this.label6.Font.FontFamily, fontSize);
            this.textBox1.Font = new Font(this.textBox1.Font.FontFamily, fontSize);
            this.textBox2.Font = new Font(this.textBox2.Font.FontFamily, fontSize);

            int labelSpacing = 10;
            int textBoxWidth = this.groupBoxApres.Width / 2 - margin * 2;

            this.label3.Left = margin;
            this.label3.Top = margin * 2;
            this.textBox1.Left = this.groupBoxApres.Width - textBoxWidth - margin;
            this.textBox1.Top = this.label3.Top;
            this.textBox1.Width = textBoxWidth;

            this.label4.Left = margin;
            this.label4.Top = this.label3.Bottom + labelSpacing;
            this.textBox2.Left = this.groupBoxApres.Width - textBoxWidth - margin;
            this.textBox2.Top = this.label4.Top;
            this.textBox2.Width = textBoxWidth;

            ConfigureAnchors();
        }

        private void FormResize_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = true;

            this.label5.Text = forme.Width.ToString();
            this.label6.Text = forme.Height.ToString();

            // Adding placeholder text to textboxes
            this.textBox1.PlaceholderText = "Enter width";
            this.textBox2.PlaceholderText = "Enter height";

            ChangeLanguage();
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int newWidth) || newWidth <= 0)
            {
                ShowErrorMessage("Veuillez entrer une largeur valide (nombre positif).");
                return;
            }

            if (!int.TryParse(textBox2.Text, out int newHeight) || newHeight <= 0)
            {
                ShowErrorMessage("Veuillez entrer une hauteur valide (nombre positif).");
                return;
            }

            forme.Size = new Size(newWidth, newHeight);

            Close();
        }

        // Method to handle error messages in multiple languages
        private void ShowErrorMessage(string message)
        {
            string errorMessage = message;

            // Adjusting the error message based on language
            switch (Forme1.Langue)
            {
                case 1: // English
                    errorMessage = "Please enter a valid positive number.";
                    break;
                case 2: // Chinese
                    errorMessage = "请输入有效的正数。";
                    break;
                case 3: // German
                    errorMessage = "Bitte geben Sie eine gültige positive Zahl ein.";
                    break;
                case 4: // French (already set as default)
                    break;
                case 5: // Lithuanian
                    errorMessage = "Prašome įvesti galiojantį teigiamą skaičių.";
                    break;
                default:
                    errorMessage = "Please enter a valid positive number.";
                    break;
            }

            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
