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
    public partial class ControlComment : Form
    {
        Control comment;
        public static string commentaire ;
        public ControlComment()
        {
            InitializeComponent();
        }

        private void ControlComment_Load(object sender, EventArgs e)
        {
            commentaire = string.Empty;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!=null)
            {
                commentaire = textBox1.Text;
                MessageBox.Show("Commentaire ajouter !","",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Close();
            }
            else
            {
                commentaire= string.Empty;
                return;
            }
        }
    }
}
