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
        int stepSize = 20;

        public FormWindowsResize(FormVide forme, Panel panel)
        {
            InitializeComponent();
            this.forme = forme;
            this.panel = panel;
            this.animationTimer = new System.Windows.Forms.Timer();
            this.animationTimer.Interval = 1;  // 20 ms pour un effet d'animation fluide
            this.animationTimer.Tick += AnimationTimer_Tick;
            this.stepSize =20;  // Pas de mouvement plus petit pour une animation plus douce
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Définir une tolérance pour arrêter l'animation
            int tolerance = 10;  // Tolérance de 2 pixels pour position et taille

            // Déplacer la forme progressivement
            if (Math.Abs(forme.Location.X - targetX) > tolerance)
                forme.Location = new Point(forme.Location.X + (forme.Location.X < targetX ? stepSize : -stepSize), forme.Location.Y);

            if (Math.Abs(forme.Location.Y - targetY) > tolerance)
                forme.Location = new Point(forme.Location.X, forme.Location.Y + (forme.Location.Y < targetY ? stepSize : -stepSize));

            // Redimensionner progressivement
            if (Math.Abs(forme.Width - targetWidth) > tolerance)
                forme.Width += (forme.Width < targetWidth ? stepSize : -stepSize);

            if (Math.Abs(forme.Height - targetHeight) > tolerance)
                forme.Height += (forme.Height < targetHeight ? stepSize : -stepSize);

            // Vérifier si l'animation est terminée avec la tolérance
            if (Math.Abs(forme.Location.X - targetX) <= tolerance &&
                Math.Abs(forme.Location.Y - targetY) <= tolerance &&
                Math.Abs(forme.Width - targetWidth) <= tolerance &&
                Math.Abs(forme.Height - targetHeight) <= tolerance)
            {
                animationTimer.Stop();  // Arrêter l'animation une fois la cible atteinte
                this.Hide();  // Cacher la fenêtre
                this.Dispose();  // Libérer les ressources de la fenêtre
            }
        }



        private void FormWindowsResize_Load(object sender, EventArgs e)
        {
            // Supprimer les bordures de la fenêtre
            this.FormBorderStyle = FormBorderStyle.None;

            // Appliquer l'événement MouseEnter uniquement aux PictureBox
            foreach (Control c in this.Controls)
            {
                if (c is PictureBox pic)
                {
                    pic.MouseEnter += C_MouseEnter;
                    pic.MouseLeave += C_MouseLeave; // Optionnel: pour restaurer la couleur de fond

                    pic.Click += Pic_Click; // Gestion du clic sur le PictureBox

                    // Utilisation de Tag pour stocker un identifiant unique
                    pic.Tag = this.Controls.GetChildIndex(pic) + 1;
                }
            }
        }

        // Gestion du clic sur un PictureBox
        private void Pic_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pic)
            {
                // Déterminer la nouvelle position et taille en fonction du tag
                if (pic.Tag.ToString() == "6")  // Pour le tag 2 (gauche)
                {
                    targetX = 0;
                    targetY = 0;
                    targetWidth = panel.Width / 2;
                    targetHeight = panel.Height;
                }
                else if (pic.Tag.ToString() == "5")  // Pour le tag 1 (droite)
                {
                    targetX = panel.Width - (panel.Width / 2);
                    targetY = 0;
                    targetWidth = panel.Width / 2;
                    targetHeight = panel.Height;
                }
                else if (pic.Tag.ToString() == "3")  // Pour le tag 1 (droite)
                {
                    targetX = panel.Width - (panel.Width / 2);
                    targetY = 0;
                    targetWidth = panel.Width / 2;
                    targetHeight = panel.Height/2;
                }
                else if (pic.Tag.ToString() == "4")  // Pour le tag 1 (droite)
                {
                    targetX = 0;
                    targetY = 0;
                    targetWidth = panel.Width / 2;
                    targetHeight = panel.Height / 2;
                }
                else if (pic.Tag.ToString() == "2")  // Pour le tag 1 (droite)
                {
                    targetX = 0;
                    targetY = panel.Height/2;
                    targetWidth = panel.Width / 2;
                    targetHeight = panel.Height / 2;
                }
                else if (pic.Tag.ToString() == "1")  // Pour le tag 1 (droite)
                {
                    targetX = panel.Width/2 ;
                    targetY = panel.Height / 2;
                    targetWidth = panel.Width / 2;
                    targetHeight = panel.Height / 2;
                }

                // Démarrer l'animation
                animationTimer.Start();
            }
        }

        // Gestion du survol sur un PictureBox
        private void C_MouseEnter(object sender, EventArgs e)
        {
            if (sender is PictureBox pic)
            {
                pic.BackColor = Color.White; // Changer la couleur de fond sur le survol
            }
        }

        // Gestion de la sortie du survol d'un PictureBox (optionnel)
        private void C_MouseLeave(object sender, EventArgs e)
        {
            if (sender is PictureBox pic)
            {
                pic.BackColor = Color.Transparent; // Restaurer la couleur de fond originale
            }
        }
    }
}
