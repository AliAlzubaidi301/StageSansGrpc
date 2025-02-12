using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static StageCode.LIB.OrthoAD;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace StageCode.LIB
{
    public partial class OrthoEdit : UserControl
    {
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        private TDesign _TypeDesign; // Type d'entrée du champ (cf. Classe TDesign)
        private string _Format = "0.00"; // Format de la variable (précision)
        private string _comment = ""; // Commentaire sur le contrôle
        private int _BorderW = 0;

        private string _VarMem; // Nom de la Variable mémoire
        private FieldType _FieldType; // Type de champ
        private string _TextVirtualKeyboard = ""; // Texte a afficher pour le clavier virtuel
        private string _ValMin, _ValMax;
        private int _MaxLength;
        private string CaraPsw;
        private string _visibility = "1";
        /// <summary>
        /// Param non utilisé mais visible au cas où
        /// </summary>
        private string Det;

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoEdit()
        {
            InitializeComponent();
            AutoSize = false;
            TextBox1.Multiline = true;
            ControlUtils.RegisterControl(TextBox1, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoEdit_Resize;
            base.Load += OrthoEdit_Load;
        }
        private void OrthoEdit_Resize(object sender, EventArgs e)
        {
            TextBox1.Size = this.Size;
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
            var StyleText = new FontStyle();

            if (bool.TryParse(splitPvirgule[9], out bool isStrikeout) && isStrikeout)
            {
                StyleText |= FontStyle.Strikeout;
            }
            if (bool.TryParse(splitPvirgule[10], out bool isUnderline) && isUnderline)
            {
                StyleText |= FontStyle.Underline;
            }
            if (bool.TryParse(splitPvirgule[11], out bool isBold) && isBold)
            {
                StyleText |= FontStyle.Bold;
            }
            if (bool.TryParse(splitPvirgule[12], out bool isItalic) && isItalic)
            {
                StyleText |= FontStyle.Italic;
            }

            this.Name = splitPvirgule[1] + "_" + splitPvirgule[2];
            Format = splitPvirgule[4];
            //Font = new Font(splitPvirgule[7], float.Parse(splitPvirgule[8]), StyleText);
            this.Text = splitPvirgule[2];
            BackColor = FromOle(splitPvirgule[5]);
            ForeColor = FromOle(splitPvirgule[6]);
            BorderWidth = int.Parse(splitPvirgule[14]);
            this.Size = new Size(int.Parse(splitPvirgule[16]), int.Parse(splitPvirgule[15]));
            if (FromCopy)
            {
                this.Location = new Point((int)Math.Round(Double.Parse(splitPvirgule[18]) + 10d), (int)Math.Round(Double.Parse(splitPvirgule[17]) + 10d));
            }
            else
            {
                this.Location = new Point(int.Parse(splitPvirgule[18]), int.Parse(splitPvirgule[17]));
            }
            ColorOn = FromOle(splitPvirgule[28]);
            ColorOff = FromOle(splitPvirgule[29]);
            ColorErr = FromOle(splitPvirgule[30]);
            LevelVisible = int.Parse(splitPvirgule[31]);
            LevelEnabled = int.Parse(splitPvirgule[32]);
            this.comment = comment;
            TextAlign = (HorizontalAlignment)(int.Parse(splitPvirgule[3]) % 10);
            VarMeM = splitPvirgule[19];
            if (Enum.TryParse(splitPvirgule[20], out FieldType result))
            {
                FieldType = result;
            }
            ValMin = splitPvirgule[21];
            ValMax = splitPvirgule[22];
            // BUG506 Version 1.0.10 AJout de MaxLength
            // BUG 1024 Version 1.0.12 Vérification champ MaxLength pas vide
            if (!string.IsNullOrEmpty(splitPvirgule[24]))
            {
                MaxLength = int.Parse(splitPvirgule[24]);
            }

            PasswordChar = splitPvirgule[25];
            TextVirtualKeyboard = splitPvirgule[26];
            Det = splitPvirgule[27];

            if (splitPvirgule.Length >= 34)
            {
                Visibility = splitPvirgule[33];
            }
            return this;
        }
        public OrthoEdit ReadFileXML(string txt)
        {
            XElement component = XElement.Parse(txt);

            var appearance = component.Element("Apparence");

            Text = appearance.Element("Text")?.Value ?? "";
            TextAlign = (HorizontalAlignment)(ContentAlignment)Enum.Parse(typeof(ContentAlignment), appearance.Element("TextAlign")?.Value ?? "MiddleCenter");
            Format = appearance.Element("Format")?.Value ?? "";
            BackColor = ColorTranslator.FromOle(int.Parse(appearance.Element("BackColor")?.Value ?? "0"));
            ForeColor = ColorTranslator.FromOle(int.Parse(appearance.Element("ForeColor")?.Value ?? "0"));
            Font = new Font(
                appearance.Element("FontName")?.Value ?? "Arial",
                float.Parse(appearance.Element("FontSize")?.Value ?? "10"),
                (FontStyle)
                ((bool.Parse(appearance.Element("FontBold")?.Value ?? "false") ? FontStyle.Bold : 0) |
                 (bool.Parse(appearance.Element("FontItalic")?.Value ?? "false") ? FontStyle.Italic : 0) |
                 (bool.Parse(appearance.Element("FontUnderline")?.Value ?? "false") ? FontStyle.Underline : 0) |
                 (bool.Parse(appearance.Element("FontStrikeout")?.Value ?? "false") ? FontStyle.Strikeout : 0))
            );

            BorderWidth = int.Parse(appearance.Element("BorderWidth")?.Value ?? "1");
            Size = new Size(
                int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),
                int.Parse(appearance.Element("SizeHeight")?.Value ?? "30")
            );
            Location = new Point(
                int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                int.Parse(appearance.Element("LocationY")?.Value ?? "0")
            );
            VarMeM = appearance.Element("VarMeM")?.Value ?? "";
            FieldType = (FieldType)Enum.Parse(typeof(FieldType), appearance.Element("FieldType")?.Value ?? "0");
            ValMin = (appearance.Element("ValMin")?.Value ?? "0");
            ValMax = (appearance.Element("ValMax")?.Value ?? "100");
            MaxLength = int.Parse(appearance.Element("MaxLength")?.Value ?? "255");
            PasswordChar = appearance.Element("PasswordChar")?.Value ?? "";
            TextVirtualKeyboard = appearance.Element("TextVirtualKeyboard")?.Value ?? "";
            Det = (appearance.Element("Det")?.Value ?? "false");

            ColorOn = ColorTranslator.FromOle(int.Parse(appearance.Element("ColorOn")?.Value ?? "0"));
            ColorOff = ColorTranslator.FromOle(int.Parse(appearance.Element("ColorOff")?.Value ?? "0"));
            ColorErr = ColorTranslator.FromOle(int.Parse(appearance.Element("ColorErr")?.Value ?? "0"));

            LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Value ?? "0");
            LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Value ?? "0");
            Visibility = (appearance.Element("Visibility")?.Value ?? "true");

            return this;
        }

        public string WriteFile()
        {
            return "ORTHO;EDIT;" + this.Text + ";" + TextAlign.ToString() + ";" + Format + ";" + ToOle(BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";5;" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + VarMeM + ";" + Convert.ToInt32(FieldType).ToString() + ";" + ValMin + ";" + ValMax + ";;" + MaxLength.ToString() + ";" + PasswordChar + ";" + TextVirtualKeyboard + ";" + Det + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("      <Apparence>");
            xmlContent.AppendLine($"        <Text>{Text}</Text>");
            xmlContent.AppendLine($"        <TextAlign>{TextAlign.ToString()}</TextAlign>");
            xmlContent.AppendLine($"        <Format>{Format}</Format>");
            xmlContent.AppendLine($"        <BackColor>{ToOle(BackColor)}</BackColor>");
            xmlContent.AppendLine($"        <ForeColor>{ToOle(ForeColor)}</ForeColor>");
            xmlContent.AppendLine($"        <FontName>{Font.Name}</FontName>");
            xmlContent.AppendLine($"        <FontSize>{Font.Size}</FontSize>");
            xmlContent.AppendLine($"        <FontStrikeout>{Font.Strikeout}</FontStrikeout>");
            xmlContent.AppendLine($"        <FontUnderline>{Font.Underline}</FontUnderline>");
            xmlContent.AppendLine($"        <FontBold>{Font.Bold}</FontBold>");
            xmlContent.AppendLine($"        <FontItalic>{Font.Italic}</FontItalic>");
            xmlContent.AppendLine($"        <BorderWidth>{BorderWidth}</BorderWidth>");
            xmlContent.AppendLine($"        <SizeHeight>{Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <SizeWidth>{Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <LocationY>{Location.Y}</LocationY>");
            xmlContent.AppendLine($"        <LocationX>{Location.X}</LocationX>");
            xmlContent.AppendLine($"        <VarMeM>{VarMeM}</VarMeM>");
            xmlContent.AppendLine($"        <FieldType>{Convert.ToInt32(FieldType)}</FieldType>");
            xmlContent.AppendLine($"        <ValMin>{ValMin}</ValMin>");
            xmlContent.AppendLine($"        <ValMax>{ValMax}</ValMax>");
            xmlContent.AppendLine($"        <MaxLength>{MaxLength}</MaxLength>");
            xmlContent.AppendLine($"        <PasswordChar>{PasswordChar}</PasswordChar>");
            xmlContent.AppendLine($"        <TextVirtualKeyboard>{TextVirtualKeyboard}</TextVirtualKeyboard>");
            xmlContent.AppendLine($"        <Det>{Det}</Det>");
            xmlContent.AppendLine($"        <ColorOn>{ToOle(ColorOn)}</ColorOn>");
            xmlContent.AppendLine($"        <ColorOff>{ToOle(ColorOff)}</ColorOff>");
            xmlContent.AppendLine($"        <ColorErr>{ToOle(ColorErr)}</ColorErr>");
            xmlContent.AppendLine($"        <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"        <LevelEnabled>{LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"        <Visibility>{Visibility}</Visibility>");
            xmlContent.AppendLine("      </Apparence>");
            xmlContent.AppendLine("    </Component>");

            return xmlContent.ToString();
        }


        #endregion

        #region TextBox Properties
        [Category("Apparence")]
        [Description("Indique la façon dont le texte doit être aligné dans le contrôle d'édition.")]
        public HorizontalAlignment TextAlign
        {
            get
            {
                return TextBox1.TextAlign;
            }
            set
            {
                TextBox1.TextAlign = value;
            }
        }
        [Category("Apparence")]
        [Description("La couleur d'arrière plan du contrôle.")]
        public Color BackColor
        {
            get
            {
                return TextBox1.BackColor;
            }
            set
            {
                TextBox1.BackColor = value;
            }
        }
        [Category("Apparence")]
        [ShowOnProtectedMode()]
        [Description("La couleur d'arrière plan du contrôle.")]
        public Font Font
        {
            get
            {
                return TextBox1.Font;
            }
            set
            {
                TextBox1.Font = value;
            }
        }
        [Category("Apparence")]
        [Description("La couleur d'arrière plan du contrôle.")]
        public Color ForeColor
        {
            get
            {
                return TextBox1.ForeColor;
            }
            set
            {
                TextBox1.ForeColor = value;
            }
        }
        #endregion

        #region Control properties
        [Category("Orthodyne")]
        [ShowOnProtectedMode()]
        [Browsable(true)]
        [Description("Longueur maximum (0 pas de limite)")]
        public int MaxLength
        {
            get
            {
                return _MaxLength;
            }
            set
            {
                _MaxLength = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Le nom de la variable mémoire")]
        public string VarMeM
        {
            get
            {
                return _VarMem;
            }
            set
            {
                _VarMem = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Valeur minimum si nécessaire")]
        public string ValMin
        {
            get
            {
                return _ValMin;
            }
            set
            {
                _ValMin = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Valeur maximum si nécessaire")]
        public string ValMax
        {
            get
            {
                return _ValMax;
            }
            set
            {
                _ValMax = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Type de champ (0:Num, 1:Alpha, 2:Alpha+Num, 3:Num:Num+Alpha")]
        public FieldType FieldType
        {
            get
            {
                return _FieldType;
            }
            set
            {
                _FieldType = value;
            }
        }
        [Category("Orthodyne")]
        [ShowOnProtectedMode()]
        [Description("Texte à afficher sur le clavier virtuel")]
        public string TextVirtualKeyboard
        {
            get
            {
                return _TextVirtualKeyboard;
            }
            set
            {
                _TextVirtualKeyboard = value;
            }
        }
        [Category("Orthodyne")]
        [ShowOnProtectedMode()]
        [Browsable(true)]
        [Description("Caractère utilisé pour le mot mot de passe (si vide, pas de mode mot de passe)")]
        public string PasswordChar
        {
            get
            {
                return CaraPsw;
            }
            set
            {
                if (value.Length > 1)
                {
                    CaraPsw = value.Substring(0, 1);
                }
                else
                {
                    CaraPsw = value;
                }
            }
        }
        #endregion

        #region Orthodyne Properties
        [Category("Apparence")]
        [Description("Taille de la bordure du contrôle")]
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
        [Description("Nom ou numéro du détecteur")]
        public string Detecteur
        {
            get
            {
                return Det;
            }
            set
            {
                Det = value;
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
        [Description("Indique la précision requise")]
        [DefaultValue("0.00")]
        public string Format
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
                return AllowDrop;
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
                return "EDIT";
            }
        }

        private void OrthoEdit_Load(object sender, EventArgs e)
        {

        }

        public Type GType()
        {
            return GetType();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
