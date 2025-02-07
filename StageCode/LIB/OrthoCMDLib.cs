using Microsoft.VisualBasic;
using StageCode.Other;
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
using static StageCode.LIB.OrthoAD;
namespace StageCode.LIB
{

    public partial class OrthoCMDLib : UserControl
    {
        private string _captionValues = "OrthoCMD";
        private string _comment = ""; // Commentaire sur le contrôle
        private CButton btn = new CButton();

        private string _Format; // Format de la variable (précision)
        private TDesign _TypeDesign = TDesign.Bouton; // Type d'entrée du champ (cf. Classe TDesign)
        private int _BorderW;

        /// <summary>
        /// VarLink1
        /// </summary>
        private string _Commande; // Commande executée lors du clic sur le bouton
        /// <summary>
        /// Configuration générique de l'objet CMD
        /// </summary>
        private string[] _VarLink = new string[9];

        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur

        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private string _visibility = "1";

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoCMDLib()
        {
            // Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent();

            Langs.LanguageChanged += LanguageChangedEventHandler;

            // Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            this.Controls.Add(btn);
            BackColor = Color.Transparent;
            btn.FillType = CButton.eFillType.Solid;
            btn.BackColor = Color.Transparent;
            btn.Shape = CButton.eShape.Rectangle;
            btn.Corners.All = 0;
            btn.Text = "OrthoCMD";
            ControlUtils.RegisterControl(btn, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoCMD_Resize;
        }

        private void OrthoCMD_Resize(object sender, EventArgs e)
        {
            if (btn.Shape == CButton.eShape.Ellipse)
            {
                btn.Size = new Size(this.Height, this.Height);
            }
            else
            {
                btn.Size = this.Size;
            }
        }

        /// <summary>
        /// Convertion des couleurs dans différents mode d'encodage
        /// </summary>
        /// <param name="DataIn">Couleur en format Ole</param>
        /// <returns>couleur au format .Net</returns>
        private Color FromOle(string DataIn)
        {
            // Gestion de la transparance à la Orthodyne
            if (Double.Parse(DataIn) == -1)
            {
                return Color.Transparent;
            }
            return ColorTranslator.FromOle(int.Parse(DataIn));
        }

        /// <summary>
        /// Convertion des couleurs dans différents mode d'encodage
        /// </summary>
        /// <param name="DataIn">Couleur en format .Net</param>
        /// <returns>couleur au format Ole</returns>
        private string ToOle(Color Datain)
        {
            if (Datain == Color.Transparent)
            {
                return (-1).ToString();
            }
            return ColorTranslator.ToOle(Datain).ToString();
        }
        #region Read/Write on .syn file

        public object ReadFile(string[] splitPvirgule, string comment, string file, bool FromCopy)
        {
            if (splitPvirgule == null || splitPvirgule.Length < 34) return this;

            FontStyle StyleText = FontStyle.Regular;
            if (bool.TryParse(splitPvirgule[9], out bool strikeout) && strikeout) StyleText |= FontStyle.Strikeout;
            if (bool.TryParse(splitPvirgule[10], out bool underline) && underline) StyleText |= FontStyle.Underline;
            if (bool.TryParse(splitPvirgule[11], out bool bold) && bold) StyleText |= FontStyle.Bold;
            if (bool.TryParse(splitPvirgule[12], out bool italic) && italic) StyleText |= FontStyle.Italic;

            Name = $"{splitPvirgule[1]}_{splitPvirgule[2]}";
            Caption = splitPvirgule[2];
            Precision = splitPvirgule[4];
            Font = new Font(splitPvirgule[7], float.TryParse(splitPvirgule[8], out float fontSize) ? fontSize : 12, StyleText);
            BackColor = FromOle(splitPvirgule[5]);
            ForeColor = FromOle(splitPvirgule[6]);
            if (Enum.TryParse(splitPvirgule[13], out TDesign typeDesignValue))
            {
                TypeDesign = typeDesignValue;
            }
            BorderWidth = int.TryParse(splitPvirgule[14], out int border) ? border : 0;
            Size = new Size(int.TryParse(splitPvirgule[16], out int width) ? width : 0, int.TryParse(splitPvirgule[15], out int height) ? height : 0);

            Location = new Point(
                int.TryParse(splitPvirgule[18], out int x) ? (FromCopy ? x + 10 : x) : 0,
                int.TryParse(splitPvirgule[17], out int y) ? (FromCopy ? y + 10 : y) : 0
            );

            ColorOn = FromOle(splitPvirgule[28]);
            ColorOff = FromOle(splitPvirgule[29]);
            ColorErr = FromOle(splitPvirgule[30]);
            LevelVisible = int.TryParse(splitPvirgule[31], out int levelVis) ? levelVis : 0;
            LevelEnabled = int.TryParse(splitPvirgule[32], out int levelEn) ? levelEn : 0;

            this.comment = comment;
            Commande = splitPvirgule[19];

            for (int i = 0; i < 8; i++)
            {
                if (splitPvirgule.Length > 20 + i)
                    _VarLink[i] = splitPvirgule[20 + i];
            }

            Visibility = splitPvirgule.Length >= 34 ? splitPvirgule[33] : Visibility;

            LanguageChangedEventHandler(Langs.CurrentLanguage);

            return this;
        }
        public static OrthoCMDLib ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);

            OrthoCMDLib orthoCMDLibControl = new OrthoCMDLib();

            // Extraire le type et le nom du composant
            orthoCMDLibControl.Name = xml.Attribute("name")?.Value;

            // Extraire les informations de la section principale
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                // Extraire les propriétés de l'apparence
                orthoCMDLibControl.Caption = appearance.Element("Caption")?.Value;

                // Utiliser ContentAlignment_Parser pour obtenir l'alignement
                int textAlignValue = int.Parse(appearance.Element("TextAlign")?.Value ?? "2");
                orthoCMDLibControl.TextAlign = ContentAlignment_Parser.Get_Alignment(textAlignValue);

                orthoCMDLibControl.Precision = appearance.Element("Precision")?.Value ?? "0";
                orthoCMDLibControl.BackColor = Color.FromName(appearance.Element("BackColor")?.Value ?? "Transparent");
                orthoCMDLibControl.ForeColor = Color.FromName(appearance.Element("ForeColor")?.Value ?? "Transparent");

                orthoCMDLibControl.Font = new Font(
                    appearance.Element("FontName")?.Value ?? "Arial",
                    float.Parse(appearance.Element("FontSize")?.Value ?? "10"),
                    (FontStyle)Enum.Parse(typeof(FontStyle), appearance.Element("FontStyle")?.Value ?? "Regular")
                );

                // Extraire TypeDesign en tant qu'énumération
                string typeDesignValue = appearance.Element("TypeDesign")?.Value ?? "0"; // Valeur par défaut
                orthoCMDLibControl.TypeDesign = (TDesign)Enum.Parse(typeof(TDesign), typeDesignValue);

                orthoCMDLibControl.BorderWidth = int.Parse(appearance.Element("BorderWidth")?.Value ?? "1");
                orthoCMDLibControl.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "100")
                );
                orthoCMDLibControl.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")
                );
                orthoCMDLibControl.Commande = appearance.Element("Commande")?.Value;

                // Extraire les VarLink
                orthoCMDLibControl._VarLink = new string[10]; // Ajuste la taille si nécessaire
                for (int i = 0; i < 10; i++)
                {
                    orthoCMDLibControl._VarLink[i] = appearance.Element($"VarLink{i}")?.Value ?? string.Empty;
                }

                orthoCMDLibControl.ColorOn = Color.FromName(appearance.Element("ColorOn")?.Value ?? "Transparent");
                orthoCMDLibControl.ColorOff = Color.FromName(appearance.Element("ColorOff")?.Value ?? "Transparent");
                orthoCMDLibControl.ColorErr = Color.FromName(appearance.Element("ColorErr")?.Value ?? "Transparent");
                orthoCMDLibControl.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Value ?? "0");
                orthoCMDLibControl.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Value ?? "0");
                orthoCMDLibControl.Visibility = appearance.Element("Visibility")?.Value ?? "Visible";
            }

            // Retourner l'objet OrthoCMDLib avec les données extraites
            return orthoCMDLibControl;
        }


        public string WriteFile()
        {
            return "ORTHO;CMD;" + Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(TextAlign).ToString() + ";" + Precision + ";" + ToOle(BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + Commande + ";" + _VarLink[0] + ";" + _VarLink[1] + ";" + _VarLink[2] + ";" + _VarLink[3] + ";" + _VarLink[4] + ";" + _VarLink[5] + ";" + _VarLink[6] + ";" + _VarLink[7] + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            // Début du composant spécifique
            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");

            // Section des propriétés du composant
            xmlContent.AppendLine($"      <Caption>{Caption}</Caption>");
            xmlContent.AppendLine($"      <TextAlign>{ContentAlignment_Parser.Get_ValueToWrite(TextAlign)}</TextAlign>");
            xmlContent.AppendLine($"      <Precision>{Precision}</Precision>");
            xmlContent.AppendLine($"      <BackColor>{ToOle(BackColor)}</BackColor>");
            xmlContent.AppendLine($"      <ForeColor>{ToOle(ForeColor)}</ForeColor>");
            xmlContent.AppendLine($"      <FontName>{Font.Name}</FontName>");
            xmlContent.AppendLine($"      <FontSize>{Font.Size}</FontSize>");
            xmlContent.AppendLine($"      <FontStrikeout>{Font.Strikeout}</FontStrikeout>");
            xmlContent.AppendLine($"      <FontUnderline>{Font.Underline}</FontUnderline>");
            xmlContent.AppendLine($"      <FontBold>{Font.Bold}</FontBold>");
            xmlContent.AppendLine($"      <FontItalic>{Font.Italic}</FontItalic>");
            xmlContent.AppendLine($"      <TypeDesign>{Convert.ToInt32(TypeDesign)}</TypeDesign>");
            xmlContent.AppendLine($"      <BorderWidth>{BorderWidth}</BorderWidth>");
            xmlContent.AppendLine($"      <SizeHeight>{Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"      <SizeWidth>{Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"      <LocationY>{Location.Y}</LocationY>");
            xmlContent.AppendLine($"      <LocationX>{Location.X}</LocationX>");
            xmlContent.AppendLine($"      <Commande>{Commande}</Commande>");

            // _VarLink values
            for (int i = 0; i < _VarLink.Length; i++)
            {
                xmlContent.AppendLine($"      <VarLink{i}>{_VarLink[i]}</VarLink{i}>");
            }

            xmlContent.AppendLine($"      <ColorOn>{ToOle(ColorOn)}</ColorOn>");
            xmlContent.AppendLine($"      <ColorOff>{ToOle(ColorOff)}</ColorOff>");
            xmlContent.AppendLine($"      <ColorErr>{ToOle(ColorErr)}</ColorErr>");
            xmlContent.AppendLine($"      <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"      <LevelEnabled>{LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"      <Visibility>{Visibility}</Visibility>");

            // Section Apparence supplémentaire
            xmlContent.AppendLine("      <Apparence>");
            xmlContent.AppendLine($"        <Backcolor value=\"{ToOle(BackColor)}\"/>");
            xmlContent.AppendLine($"        <FontSize value=\"{Font.Size}\"/>");
            xmlContent.AppendLine($"        <FontName value=\"{Font.Name}\"/>");
            xmlContent.AppendLine($"        <LevelVisible value=\"{LevelVisible}\"/>");
            xmlContent.AppendLine($"        <LevelEnabled value=\"{LevelEnabled}\"/>");
            xmlContent.AppendLine($"        <Visibility value=\"{Visibility}\"/>");
            xmlContent.AppendLine("      </Apparence>");

            // Fermeture du composant
            xmlContent.AppendLine("    </Component>");

            // Retourner le contenu XML généré
            return xmlContent.ToString();
        }

        #endregion

        #region Control Properties
        [Category("Apparence")]
        [Description("Alignement du texte. Note: Ne sera pas pris en compte dans Chromdyne.")]
        public ContentAlignment TextAlign
        {
            get
            {
                return btn.TextAlign;
            }
            set
            {
                btn.TextAlign = value;
            }
        }
        [Category("Apparence")]
        [Description("Caption du bouton")]
        public string Caption
        {
            get
            {
                return _captionValues;
            }
            set
            {
                _captionValues = value;
                LanguageChangedEventHandler(Langs.CurrentLanguage);
            }
        }
        public Font Font
        {
            get
            {
                return btn.Font;
            }
            set
            {
                btn.Font = value;
            }
        }
        public Color BackColor
        {
            get
            {
                return btn.ColorFillSolid;
            }
            set
            {
                btn.ColorFillSolid = value;
            }
        }
        public Color ForeColor
        {
            get
            {
                return btn.ForeColor;
            }
            set
            {
                btn.ForeColor = value;
            }
        }


        [Category("Orthodyne")]
        [Description("Commande envoyée lors du clic sur le bouton")]
        [DefaultValue("")]
        public string Commande
        {
            get
            {
                return _Commande;
            }
            set
            {
                _Commande = value;
            }
        }

        [Category("VarLink")]
        [Description("Option 2")]
        public string VarLink2
        {
            get
            {
                return _VarLink[0];
            }
            set
            {
                _VarLink[0] = value;
            }
        }
        [Category("VarLink")]
        [Description("Option 3")]
        public string VarLink3
        {
            get
            {
                return _VarLink[1];
            }
            set
            {
                _VarLink[1] = value;
            }
        }
        [Category("VarLink")]
        [Description("Option 4")]
        public string VarLink4
        {
            get
            {
                return _VarLink[2];
            }
            set
            {
                _VarLink[2] = value;
            }
        }
        [Category("VarLink")]
        [Description("Option 5")]
        public string VarLink5
        {
            get
            {
                return _VarLink[3];
            }
            set
            {
                _VarLink[3] = value;
            }
        }
        [Category("VarLink")]
        [Description("Option 6")]
        public string VarLink6
        {
            get
            {
                return _VarLink[4];
            }
            set
            {
                _VarLink[4] = value;
            }
        }
        [Category("VarLink")]
        [Description("Option 7")]
        public string VarLink7
        {
            get
            {
                return _VarLink[5];
            }
            set
            {
                _VarLink[5] = value;
            }
        }
        [Category("VarLink")]
        [Description("Option 8")]
        public string VarLink8
        {
            get
            {
                return _VarLink[6];
            }
            set
            {
                _VarLink[6] = value;
            }
        }
        [Category("VarLink")]
        [Description("Option 9")]
        public string VarLink9
        {
            get
            {
                return _VarLink[7];
            }
            set
            {
                _VarLink[7] = value;
            }
        }

        #endregion

        #region Orthodyne Properties
        [Category("Apparence")]
        [Description("Largeur de la bordure du contrôle")]
        public int BorderWidth
        {
            get
            {
                return _BorderW;
            }
            set
            {
                _BorderW = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Indique la précision requise")]
        [DisplayName("Format")]
        [DefaultValue("")]
        public string Precision
        {
            get
            {
                return _Format;
            }
            set
            {
                _Format = value;
            }
        }
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
        [Description("Indique quel type de design utiliser")]
        public TDesign TypeDesign
        {
            get
            {
                return _TypeDesign;
            }
            set
            {
                _TypeDesign = value;
                if (value == TDesign.Bouton)
                {
                    btn.Shape = CButton.eShape.Rectangle;
                    btn.Corners.All = 0;
                }
                else if (value == TDesign.Arrondi)
                {
                    btn.Shape = CButton.eShape.Rectangle;
                    btn.Corners.All = 6;
                }
                else if (value == TDesign.Rectangle)
                {
                    btn.Shape = CButton.eShape.Rectangle;
                    btn.Corners.All = 0;
                }
                else if (value == TDesign.Cercle)
                {
                    btn.Shape = CButton.eShape.Ellipse;
                    btn.Size = new Size(this.Height, this.Height);
                    this.Size = btn.Size;
                }
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
        [Description("Couleur lorsque actif")]
        public Color ColorOn
        {
            get
            {
                return _ColorOn;
            }
            set
            {
                _ColorOn = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Couleur lorsque non actif")]
        public Color ColorOff
        {
            get
            {
                return _ColorOff;
            }
            set
            {
                _ColorOff = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Couleur lorsque en erreur")]
        public Color ColorErr
        {
            get
            {
                return _ColorErr;
            }
            set
            {
                _ColorErr = value;
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
                return BackgroundImageLayout;
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
                return BorderStyle;
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
                return Cursor;
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
                return RightToLeft;
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
                return UseWaitCursor;
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
                return  ContextMenuStrip;
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
                return "ORTHO";
            }
        }

        public string SType
        {
            get
            {
                return "CMD";
            }
        }

        public Type GType()
        {
            return GetType();
        }

        private void LanguageChangedEventHandler(AvailableLanguage NewLanguage)
        {
            if (_captionValues.Contains("|"))
            {
                string[] str = _captionValues.Split("|");
                var idx = NewLanguage.LanguageID;
                if (str.Length > idx)
                {
                    btn.Text = str[idx];

                    //Ajouter OrthoLib
                }
                else
                {
                    btn.Text = str[0];
                }
            }
            else
            {
                btn.Text = _captionValues;
            }
        }

        private void OrthoCMDLib_Load(object sender, EventArgs e)
        {

        }
    }
}
