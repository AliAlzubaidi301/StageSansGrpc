using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrthoDesigner
{
    public partial class FormVide : Form
    {
        // se formulaire est affichier sur le pnlviewHost panel
        public FormVide()
        {
            InitializeComponent();
        }

        private void FormVide_Load(object sender, EventArgs e)
        {
            this.ClientSizeChanged += FormVide_ClientSizeChanged;
        }

        private void FormVide_ClientSizeChanged(object? sender, EventArgs e)
        {
            this.panel1.Size = this.ClientSize;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
