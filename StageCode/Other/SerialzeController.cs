using StageCode;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OrthoDesigner.Other
{
    public class CopyAndPasteAndCut
    {
        public static PictureBox? Picturebox { get; private set; }
        public static bool Copier = false;
        public static Control ctrl;
        public void Copy(PictureBox pic)
        {
            Picturebox = new PictureBox();

            ctrl = pic.Controls[0];
            
            Copier = true;
        }

        public void Cut(PictureBox pic)
        {
            Picturebox = pic;
            Forme1.forme.panel1.Controls.Remove(pic);
            Copier = false;
        }

        public void Paste(Point eLocation)
        {
            if (Picturebox != null)
            {
                PictureBox newPic = Picturebox;
                newPic.Location = eLocation;
                if(!Copier)
                Forme1.forme.panel1.Controls.Add(newPic);
            }
        }
    }
}
