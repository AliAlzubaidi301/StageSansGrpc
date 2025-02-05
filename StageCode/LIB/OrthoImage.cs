using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageCode.LIB
{
    public partial class OrthoImage : UserControl
    {
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private string _comment = ""; // Commentaire sur le contrôle

        public string _path;
        private string _imglocation;
        private BorderStyle _borderstyle;
        private string _visibility = "1";

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoImage()
        {
            InitializeComponent();
            PictureBox1.BackColor = Color.White;
            // Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            ControlUtils.RegisterControl(PictureBox1, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoImage_Resize;
        }

        private void OrthoImage_Load(object sender, EventArgs e)
        {

        }
        #region Read/Write on .syn file
        public object ReadFile(string[] splitPvirgule, string comment, string file, bool FromCopy)
        {
            _path = file;
            this.comment = comment;
            this.Name = splitPvirgule[0] + "_" + splitPvirgule[1];
            // BUG 1030 : Le chemin d'accès est juste le nom de l'image. On rajoutera le chemin absolu lors de la création de la PictureBox
            // Me.ImageLocation = _path.Substring(0, _path.LastIndexOf("\")) + "\Pictures\" + splitPvirgule(1)
            ImageLocation = splitPvirgule[1];
            this.Size = new Size(int.Parse(splitPvirgule[4]), int.Parse(splitPvirgule[3]));
            if (FromCopy)
            {
                this.Location = new Point((int)Math.Round(Double.Parse(splitPvirgule[6]) + 10d), (int)Math.Round(Double.Parse(splitPvirgule[5]) + 10d));
            }
            else
            {
                this.Location = new Point(int.Parse(splitPvirgule[6]), int.Parse(splitPvirgule[5]));
            }
            LevelVisible = int.Parse(splitPvirgule[7]);
            LevelEnabled = int.Parse(splitPvirgule[8]);
            AutoSize = true;
            if (Double.Parse(splitPvirgule[2]) == 0d)
            {
                this.BorderStyle = BorderStyle.None;
            }
            else if (Double.Parse(splitPvirgule[2]) == 1d)
            {
                this.BorderStyle = BorderStyle.FixedSingle;
            }

            if (splitPvirgule.Length >= 10)
            {
                Visibility = splitPvirgule[9];
            }

            return this;
        }

        public string WriteFile()
        {
            int bs;
            if (this.BorderStyle == BorderStyle.None)
            {
                bs = 0;
            }
            else
            {
                bs = 1;
            }
            // Bug:172 correction chemin image
            // Return "IMAGE;" + System.IO.Path.GetFileName(Me.ImageLocation) + ";" + bs.ToString() + ";" + Me.Size.Height.ToString() + ";" + Me.Size.Width.ToString() + ";" + Me.Location.Y.ToString() + ";" + Me.Location.X.ToString() + ";" + Me.LevelVisible.ToString() + ";" + Me.LevelEnabled.ToString()
            // BUG 1030 : L'image ne retient que le nom du fichier, puisque les images sont dans Syno/Pictures/
            return "IMAGE;" + ImageLocation + ";" + bs.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("      <Apparence>");
            xmlContent.AppendLine($"        <ImageLocation>{ImageLocation}</ImageLocation>");

            // Handling the BorderStyle as a boolean value for the XML (0 for None, 1 for other types)
            int bs = (this.BorderStyle == BorderStyle.None) ? 0 : 1;
            xmlContent.AppendLine($"        <BorderStyle>{bs}</BorderStyle>");

            xmlContent.AppendLine($"        <SizeHeight>{Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <SizeWidth>{Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <LocationY>{Location.Y}</LocationY>");
            xmlContent.AppendLine($"        <LocationX>{Location.X}</LocationX>");
            xmlContent.AppendLine($"        <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"        <LevelEnabled>{LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"        <Visibility>{Visibility}</Visibility>");

            xmlContent.AppendLine("      </Apparence>");
            xmlContent.AppendLine("    </Component>");

            return xmlContent.ToString();
        }

        #endregion

        #region Control Properties
        [Category("Orthodyne")]
        public string ImageLocation
        {
            get
            {
                return _imglocation;
            }
            set
            {
                _imglocation = value;
                try
                {
                    // BUG 1030 : Value ne contient plus que le nom du fichier, on rajoute donc le chemin complet
                    // PictureBox1.Image = System.Drawing.Image.FromFile(value)
                    PictureBox1.Image = System.Drawing.Image.FromFile(_path.Substring(0, _path.LastIndexOf(@"\")) + @"\Pictures\" + value);
                }
                catch
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        // BUG 1031 : Ajout du path complet de l'image en cas d'erreur
                        MessageBox.Show("Can't find this image :" + Constants.vbCrLf + _path.Substring(0, _path.LastIndexOf(@"\")) + @"\Pictures\" + value);
                    }
                }
            }
        }
        #endregion

        #region Orthodyne Properties
        [Category("Orthodyne")]
        [Browsable(false)]
        [Description("Commentaire sur l'objet")]
        public string comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        [Category("Orthodyne")]
        [Description("Niveau minimum pour rendre l'objet visible (si 0, toujours visible)")]
        public int LevelVisible
        {
            get
            {
                return _LevelVisible;
            }
            set
            {
                _LevelVisible = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Niveau minimum pour rendre l'objet accessible (si 0, toujours accessible)")]
        public int LevelEnabled
        {
            get
            {
                return _LevelEnabled;
            }
            set
            {
                _LevelEnabled = value;
            }
        }

        [Category("Orthodyne")]
        [Description("If 0 or will be hidden, if #VarName will depend on variable value")]
        public string Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {

                VisibilityChanging?.Invoke(this, EventArgs.Empty);
                if (string.IsNullOrEmpty(value))
                    _visibility = "1";
                else
                    _visibility = value;
                VisibilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Hiding useless Properties
        [Browsable(false)]
        public Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }
        [Browsable(false)]
        public Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }
        [Browsable(false)]
        public Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }
        [Browsable(false)]
        public AccessibleRole AccessibleRole
        {
            get
            {
                return base.AccessibleRole;
            }
            set
            {
                base.AccessibleRole = value;
            }
        }
        [Browsable(false)]
        public string AccessibleDescription
        {
            get
            {
                return AccessibleDescription;
            }
            set
            {
                base.AccessibleDescription = value;
            }
        }
        [Browsable(false)]
        public string AccessibleName
        {
            get
            {
                return AccessibleName;
            }
            set
            {
                base.AccessibleName = value;
            }
        }
        [Browsable(false)]
        public Image BackgroundImage
        {
            get
            {
                return BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }
        [Browsable(false)]
        public ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }
        [Browsable(false)]
        public Cursor Cursor
        {
            get
            {
                return base.Cursor;
            }
            set
            {
                base.Cursor = value;
            }
        }
        [Browsable(false)]
        public RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
            }
        }
        [Browsable(false)]
        public bool UseWaitCursor
        {
            get
            {
                return base.UseWaitCursor;
            }
            set
            {
                base.UseWaitCursor = value;
            }
        }
        [Browsable(false)]
        public bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;
            }
        }
        [Browsable(false)]
        public AutoValidate AutoValidate
        {
            get
            {
                return base.AutoValidate;
            }
            set
            {
                base.AutoValidate = value;
            }
        }
        [Browsable(false)]
        public ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }
        [Browsable(false)]
        public bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }
        [Browsable(false)]
        public ImeMode ImeMode
        {
            get
            {
                return base.ImeMode;
            }
            set
            {
                base.ImeMode = value;
            }
        }
        [Browsable(false)]
        public int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            set
            {
                base.TabIndex = value;
            }
        }
        [Browsable(false)]
        public bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = value;
            }
        }
        [Browsable(false)]
        public bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }
        [Browsable(false)]
        public AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }
        [Browsable(false)]
        public bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
            set
            {
                base.AutoScroll = value;
            }
        }
        [Browsable(false)]
        public Size AutoScrollMargin
        {
            get
            {
                return base.AutoScrollMargin;
            }
            set
            {
                base.AutoScrollMargin = value;
            }
        }
        [Browsable(false)]
        public Size AutoScrollMinSize
        {
            get
            {
                return base.AutoScrollMinSize;
            }
            set
            {
                base.AutoScrollMinSize = value;
            }
        }
        [Browsable(false)]
        public bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }
        [Browsable(false)]
        public AutoSizeMode AutoSizeMode
        {
            get
            {
                return base.AutoSizeMode;
            }
            set
            {
                base.AutoSizeMode = value;
            }
        }
        [Browsable(false)]
        public DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = value;
            }
        }
        [Browsable(false)]
        public Padding Margin
        {
            get
            {
                return base.Margin;
            }
            set
            {
                base.Margin = value;
            }
        }
        [Browsable(false)]
        public Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
            set
            {
                base.MaximumSize = value;
            }
        }
        [Browsable(false)]
        public Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = value;
            }
        }
        [Browsable(false)]
        public Padding Padding
        {
            get
            {
                return base.Padding;
            }
            set
            {
                base.Padding = value;
            }
        }
        [Browsable(false)]
        public ControlBindingsCollection DataBindings
        {
            get
            {
                return base.DataBindings;
            }
        }
        [Browsable(false)]
        public object Tag
        {
            get
            {
                return Tag;
            }
            set
            {
                base.Tag = value;
            }
        }
        [Browsable(false)]
        public bool CausesValidation
        {
            get
            {
                return base.CausesValidation;
            }
            set
            {
                base.CausesValidation = value;
            }
        }
        #endregion

        private void OrthoImage_Resize(object sender, EventArgs e)
        {
        }

        public string Type
        {
            get
            {
                return "IMAGE";
            }
        }

        public string SType
        {
            get
            {
                return "IMAGE";
            }
        }

        public Type GType()
        {
            return GetType();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
