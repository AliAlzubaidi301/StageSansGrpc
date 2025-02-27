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
using System.Xml.Linq;

namespace StageCode.LIB
{
    public partial class Reticule : UserControl
    {

        #region Orthodyn Data
        private string _comment = ""; // Commentaire sur le contrôle
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private string _visibility = "1";
        #endregion

        #region Control Data
        private Label[] texte = new Label[8];
        private string _Detecteur;
        #endregion

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;
        public Reticule()
        {
            InitializeComponent();

            for (int i = 0; i <= 6; i++)
            {
                texte[i] = new Label();
                // Me.Controls.Add(texte(i))
                texte[i].AutoSize = true;
                texte[i].TextAlign = ContentAlignment.MiddleLeft;
                this.Controls.Add(texte[i]);
            }
            texte[0].Text = "LabX";
            texte[1].Text = "UnitX";
            texte[1].TextAlign = ContentAlignment.MiddleRight;
            texte[2].Text = "LabY";
            texte[3].Text = "UnitY";
            texte[3].TextAlign = ContentAlignment.MiddleRight;
            texte[4].Text = "LabS";
            texte[5].Text = "UnitS";
            texte[5].TextAlign = ContentAlignment.MiddleRight;
            texte[6].Text = "LabW";
            // Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            ControlUtils.RegisterControl(this, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += Reticule_Resize;
            base.Load += Reticule_Load;
        }

        private void Reticule_Load(object sender, EventArgs e)
        {

        }


        private void Reticule_Resize(object sender, EventArgs e)
        {
            texte[1].Location = new Point(texte[0].Size.Width + 5, texte[0].Location.Y);
            texte[2].Location = new Point(this.Size.Width / 4, texte[0].Location.Y);
            texte[3].Location = new Point(texte[2].Location.X + texte[2].Size.Width + 5, texte[0].Location.Y);
            texte[4].Location = new Point(this.Size.Width / 2, texte[0].Location.Y);
            texte[5].Location = new Point(texte[4].Location.X + texte[4].Size.Width + 5, texte[0].Location.Y);
            texte[6].Location = new Point(this.Size.Width / 4 * 3, texte[0].Location.Y);
        }
        #region Read/Write on .syn file
        public object ReadFile(string[] splitPvirgule, string comment, string file, bool FromCopy)
        {
            this.Name = splitPvirgule[1];

            this.Size = new Size(int.Parse(splitPvirgule[3]), int.Parse(splitPvirgule[2]));
            if (FromCopy)
            {
                this.Location = new Point((int)Math.Round(Double.Parse(splitPvirgule[5]) + 10d), (int)Math.Round(Double.Parse(splitPvirgule[4]) + 10d));
            }
            else
            {
                this.Location = new Point(int.Parse(splitPvirgule[5]), int.Parse(splitPvirgule[4]));
            }
            this.comment = comment;
            Detecteur = splitPvirgule[6];
            LabX = splitPvirgule[7];
            UnitX = splitPvirgule[8];
            LabY = splitPvirgule[9];
            UnitY = splitPvirgule[10];
            LabS = splitPvirgule[11];
            UnitS = splitPvirgule[12];
            LabW = splitPvirgule[13];
            _LevelVisible = int.Parse(splitPvirgule[14]);
            _LevelEnabled = int.Parse(splitPvirgule[15]);

            if (splitPvirgule.Length >= 17)
            {
                Visibility = splitPvirgule[16];
            }
            return this;
        }
        public Reticule ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);
            Reticule reticuleControl = new Reticule();

            // Trouver l'élément <Component> avec un type spécifique (ici "Reticule") et son nom
            XElement componentElement = xml.Descendants("Component")
                                           .FirstOrDefault(c => c.Attribute("type")?.Value == "Reticule");

            if (componentElement != null)
            {
                // Extraire le nom du composant
                reticuleControl.Name = componentElement.Attribute("name")?.Value;

                // Extraire la section <Reticule> qui se trouve sous <Component>
                XElement reticuleElement = componentElement.Element("Reticule");
                if (reticuleElement != null)
                {
                    // Extraire les propriétés générales
                    reticuleControl.Size = new Size(
                        int.Parse(reticuleElement.Element("SizeWidth")?.Value ?? "100"),
                        int.Parse(reticuleElement.Element("SizeHeight")?.Value ?? "100")
                    );
                    reticuleControl.Location = new Point(
                        int.Parse(reticuleElement.Element("LocationX")?.Value ?? "0"),
                        int.Parse(reticuleElement.Element("LocationY")?.Value ?? "0")
                    );
                    reticuleControl.Detecteur = reticuleElement.Element("Detecteur")?.Value ?? string.Empty;
                    reticuleControl.LabX = reticuleElement.Element("LabX")?.Value ?? string.Empty;
                    reticuleControl.UnitX = reticuleElement.Element("UnitX")?.Value ?? string.Empty;
                    reticuleControl.LabY = reticuleElement.Element("LabY")?.Value ?? string.Empty;
                    reticuleControl.UnitY = reticuleElement.Element("UnitY")?.Value ?? string.Empty;
                    reticuleControl.LabS = reticuleElement.Element("LabS")?.Value ?? string.Empty;
                    reticuleControl.UnitS = reticuleElement.Element("UnitS")?.Value ?? string.Empty;
                    reticuleControl.LabW = reticuleElement.Element("LabW")?.Value ?? string.Empty;

                    // Extraire les niveaux de visibilité et d'activation
                    reticuleControl.LevelVisible = int.Parse(reticuleElement.Element("LevelVisible")?.Value ?? "0");
                    reticuleControl.LevelEnabled = int.Parse(reticuleElement.Element("LevelEnabled")?.Value ?? "0");

                    // Extraire la visibilité
                    reticuleControl.Visibility = reticuleElement.Element("Visibility")?.Value ?? "Visible";
                }
            }

            return reticuleControl;
        }

        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("      <Reticule>");

            // Properties
            xmlContent.AppendLine($"        <Name>{this.Name}</Name>");
            xmlContent.AppendLine($"        <SizeHeight>{this.Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <SizeWidth>{this.Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <LocationY>{this.Location.Y}</LocationY>");
            xmlContent.AppendLine($"        <LocationX>{this.Location.X}</LocationX>");
            xmlContent.AppendLine($"        <Detecteur>{Detecteur}</Detecteur>");
            xmlContent.AppendLine($"        <LabX>{LabX}</LabX>");
            xmlContent.AppendLine($"        <UnitX>{UnitX}</UnitX>");
            xmlContent.AppendLine($"        <LabY>{LabY}</LabY>");
            xmlContent.AppendLine($"        <UnitY>{UnitY}</UnitY>");
            xmlContent.AppendLine($"        <LabS>{LabS}</LabS>");
            xmlContent.AppendLine($"        <UnitS>{UnitS}</UnitS>");
            xmlContent.AppendLine($"        <LabW>{LabW}</LabW>");
            xmlContent.AppendLine($"        <LevelVisible>{_LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"        <LevelEnabled>{_LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"        <Visibility>{Visibility}</Visibility>");

            xmlContent.AppendLine("      </Reticule>");
            xmlContent.AppendLine("    </Component>");

            return xmlContent.ToString();
        }

        public string WriteFile()
        {
            return "RETICULE;" + this.Name + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + Detecteur + ";" + LabX + ";" + UnitX + ";" + LabY + ";" + UnitY + ";" + LabS + ";" + UnitS + ";" + LabW + ";" + _LevelVisible.ToString() + ";" + _LevelEnabled.ToString() + ";" + Visibility;
        }
        #endregion

        #region Control Properties
        [Category("Orthodyne")]
        [Description("Nom du détecteur associé au graph")]
        public string Detecteur
        {
            get
            {
                return _Detecteur;
            }
            set
            {
                _Detecteur = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Label de l'Axe X")]
        public string LabX
        {
            get
            {
                return texte[0].Text;
            }
            set
            {
                texte[0].Text = value;

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
        [Description("Label de l'Axe Y")]
        public string LabY
        {
            get
            {
                return texte[2].Text;
            }
            set
            {
                texte[2].Text = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Label du Slope")]
        public string LabS
        {
            get
            {
                return texte[4].Text;
            }
            set
            {
                texte[4].Text = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Label de W")]
        public string LabW
        {
            get
            {
                return texte[6].Text;
            }
            set
            {
                texte[6].Text = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Unité de grandeur de l'Axe X")]
        public string UnitX
        {
            get
            {
                return texte[1].Text;
            }
            set
            {
                texte[1].Text = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Unité de grandeur de l'Axe Y")]
        public string UnitY
        {
            get
            {
                return texte[3].Text;
            }
            set
            {
                texte[3].Text = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Unité de grandeur du Slope")]
        public string UnitS
        {
            get
            {
                return texte[5].Text;
            }
            set
            {
                texte[5].Text = value;
            }
        }
        [Browsable(false)]
        public Color BackColor
        {
            get
            {
                return BackColor;
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
                return ForeColor;
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
                return Font;
            }
            set
            {
                base.Font = value;
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
        public BorderStyle BorderStyle
        {
            get
            {
                return base.BorderStyle;
            }
            set
            {
                base.BorderStyle = value;
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
                return Enabled;
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
                return ImeMode;
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
                return TabIndex;
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
                return TabStop;
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
                return CausesValidation;
            }
            set
            {
                base.CausesValidation = value;
            }
        }
        #endregion

        public string Type
        {
            get
            {
                return "RETICULE";
            }
        }

        public string SType
        {
            get
            {
                return "RETICULE";
            }
        }

        public Type GType()
        {
            return GetType();
        }
    }
}