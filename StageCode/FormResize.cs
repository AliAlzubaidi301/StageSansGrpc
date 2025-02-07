using System;
using System.Drawing;
using System.Windows.Forms;

namespace StageCode
{
    public partial class FormResize : Form
    {
        private Form1 forme;

        public FormResize(Form1 forme)
        {
            InitializeComponent();
            this.forme = forme;

            ConfigureAnchors();

            this.SizeChanged += FormResize_SizeChanged;
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

            // Ajuster la taille de police
            float fontSize = Math.Max(8, this.ClientSize.Width / 50);
            this.label1.Font = new Font(this.label1.Font.FontFamily, fontSize);
            this.label2.Font = new Font(this.label2.Font.FontFamily, fontSize);
            this.label5.Font = new Font(this.label5.Font.FontFamily, fontSize);
            this.label6.Font = new Font(this.label6.Font.FontFamily, fontSize);
            this.textBox1.Font = new Font(this.textBox1.Font.FontFamily, fontSize);
            this.textBox2.Font = new Font(this.textBox2.Font.FontFamily, fontSize);

            // Repositionner les labels et textboxes
            int labelSpacing = 10;
            int textBoxWidth = this.groupBoxApres.Width / 2 - margin * 2;

            this.label3.Left = margin;
            this.label3.Top = margin*2;
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

            this.groupBoxAvant.Text = "Old Size";
            this.groupBoxApres.Text = "New Size";

            this.label1.Text = "Width";
            this.label2.Text = "Height";

            label3.Text = "Width";
            label4.Text = "Height";

            this.label5.Text = forme.Width.ToString();
            this.label6.Text = forme.Height.ToString();

            this.textBox1.Text = forme.Width.ToString();
            this.textBox2.Text = forme.Height.ToString();

            this.btnOK.Text = "OK";
            this.btnResize.Text = "Cancel";
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            // Annuler
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int newWidth) || newWidth <= 0)
            {
                MessageBox.Show("Veuillez entrer une largeur valide (nombre positif).", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox2.Text, out int newHeight) || newHeight <= 0)
            {
                MessageBox.Show("Veuillez entrer une hauteur valide (nombre positif).", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            forme.Size = new Size(newWidth, newHeight);

            Close();
        }
    }
}