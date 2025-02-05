using Microsoft.VisualBasic;
using StageCode.LIB;
using StageCode.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static StageCode.LIB.OrthoAD;
using static StageCode.Other.Langs;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace StageCode.LIB
{
    public partial class OrthoRel : UserControl
    {

        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        private TDesign _TypeDesign = TDesign.Bouton;  // Type d'entrée du champ (cf. Classe TDesign)
        private string _Format; // Format de la variable (précision)
        private int _BorderW;
        private string _comment = ""; // Commentaire sur le contrôle

        private CButton btn = new CButton();
        private string _Variable; // Nom de la variable qui contient le texte a afficher
        private string _TextOff; // Texte a afficher si relais off
        private string _TextOn; // Texte a afficher si relais on
        private ModeRelais _ModeRelais;
        private string _captionValues = "OrthoRelay";
        private string _visibility = "1";

        /// <summary>
        /// Param non utilisé mais visible au cas où
        /// </summary>
        private string[] _VL = new string[9];

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoRel()
        {
            InitializeComponent();
            this.Controls.Add(btn);
            btn.FillType = CButton.eFillType.Solid;
            btn.BackColor = Color.Transparent;
            BackColor = Color.Transparent;
            Caption = "OrthoRel";
            btn.Shape = CButton.eShape.Rectangle;
            btn.Corners.All = 0;
            // Ajoutez une initialisation quelconque après l'appel InitializeComponent().

            Langs.LanguageChanged += LanguageChangedEventHandler;
            LanguageChangedEventHandler(Langs.CurrentLanguage);
            ControlUtils.RegisterControl(btn, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoRel_Resize;
            base.Load += OrthoRel_Load;
        }

        private void OrthoRel_Load(object sender, EventArgs e)
        {

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

        private void OrthoRel_Resize(object sender, EventArgs e)
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
            var StyleText = new FontStyle();

            TextAlign = ContentAlignment_Parser.Get_Alignment(int.Parse(splitPvirgule[3]));

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
            Caption = splitPvirgule[2];
            Precision = splitPvirgule[4];
            Font = new Font(splitPvirgule[7], float.Parse(splitPvirgule[8]), StyleText);
            this.Text = splitPvirgule[2];
            BackColor = FromOle(splitPvirgule[5]);
            ForeColor = FromOle(splitPvirgule[6]);
            TypeDesign = (TDesign)Enum.Parse(typeof(TDesign), splitPvirgule[13]);
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
            Variable = splitPvirgule[19];
            if (int.TryParse(splitPvirgule[20], out int relaisMode))
            {
                RelaisMode = (ModeRelais)relaisMode;
            }
            TextOff = splitPvirgule[22];
            TextOn = splitPvirgule[23];
            _VL[5] = splitPvirgule[24];
            _VL[6] = splitPvirgule[25];
            _VL[7] = splitPvirgule[26];
            _VL[8] = splitPvirgule[27];

            if (splitPvirgule.Length >= 34)
            {
                Visibility = splitPvirgule[33];
            }

            return this;
        }

        public string WriteFile()
        {
            return "ORTHO;REL;" + Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(TextAlign).ToString() + ";" + Precision + ";" + ToOle(BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + Variable + ";" + Convert.ToInt32((int)RelaisMode).ToString() + ";;" + TextOff + ";" + TextOn + ";" + _VL[5] + ";" + _VL[6] + ";" + _VL[7] + ";" + _VL[8] + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"<Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("  <Apparence>");

            // Properties
            xmlContent.AppendLine($"    <Caption>{Caption}</Caption>");
            xmlContent.AppendLine($"    <TextAlign>{ContentAlignment_Parser.Get_ValueToWrite(TextAlign)}</TextAlign>");
            xmlContent.AppendLine($"    <Precision>{Precision}</Precision>");
            xmlContent.AppendLine($"    <BackColor>{ToOle(BackColor)}</BackColor>");
            xmlContent.AppendLine($"    <ForeColor>{ToOle(ForeColor)}</ForeColor>");
            xmlContent.AppendLine($"    <FontName>{Font.Name}</FontName>");
            xmlContent.AppendLine($"    <FontSize>{Font.Size}</FontSize>");
            xmlContent.AppendLine($"    <FontStrikeout>{Font.Strikeout}</FontStrikeout>");
            xmlContent.AppendLine($"    <FontUnderline>{Font.Underline}</FontUnderline>");
            xmlContent.AppendLine($"    <FontBold>{Font.Bold}</FontBold>");
            xmlContent.AppendLine($"    <FontItalic>{Font.Italic}</FontItalic>");
            xmlContent.AppendLine($"    <TypeDesign>{Convert.ToInt32(TypeDesign)}</TypeDesign>");
            xmlContent.AppendLine($"    <BorderWidth>{BorderWidth}</BorderWidth>");
            xmlContent.AppendLine($"    <SizeHeight>{Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"    <SizeWidth>{Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"    <LocationY>{Location.Y}</LocationY>");
            xmlContent.AppendLine($"    <LocationX>{Location.X}</LocationX>");
            xmlContent.AppendLine($"    <Variable>{Variable}</Variable>");
            xmlContent.AppendLine($"    <RelaisMode>{Convert.ToInt32((int)RelaisMode)}</RelaisMode>");
            xmlContent.AppendLine($"    <TextOff>{TextOff}</TextOff>");
            xmlContent.AppendLine($"    <TextOn>{TextOn}</TextOn>");

            // _VL values
            for (int i = 5; i < _VL.Length; i++)
            {
                xmlContent.AppendLine($"    <VL{i}>{_VL[i]}</VL{i}>");
            }

            xmlContent.AppendLine($"    <ColorOn>{ToOle(ColorOn)}</ColorOn>");
            xmlContent.AppendLine($"    <ColorOff>{ToOle(ColorOff)}</ColorOff>");
            xmlContent.AppendLine($"    <ColorErr>{ToOle(ColorErr)}</ColorErr>");
            xmlContent.AppendLine($"    <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"    <LevelEnabled>{LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"    <Visibility>{Visibility}</Visibility>");
            xmlContent.AppendLine("  </Apparence>");
            xmlContent.AppendLine("</Component>");

            return xmlContent.ToString();
        }

        #endregion

        #region Children Properties
        [Category("Apparence")]
        [Description("Alignement du texte")]
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
        [ShowOnProtectedMode()]
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
        [Category("Apparence")]
        [Description("Caption du relais")]
        [ShowOnProtectedMode()]
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
        #endregion

        #region Control Properties
        [Category("Orthodyne")]
        [Description("Variable qui donne le texte à afficher")]
        public string Variable
        {
            get
            {
                return _Variable;
            }
            set
            {
                _Variable = value;
            }
        }
        [Category("Orthodyne")]
        [ShowOnProtectedMode()]
        [Description("Texte si le relais est Off (si vide, prend la valeur de Text)")]
        public string TextOff
        {
            get
            {
                return _TextOff;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _TextOff = this.Text;
                }
                else
                {
                    _TextOff = value;
                }
            }
        }
        [Category("Orthodyne")]
        [ShowOnProtectedMode()]
        [Description("Texte si le relais est On (si vide, prend la valeur de Text)")]
        public string TextOn
        {
            get
            {
                return _TextOn;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _TextOn = this.Text;
                }
                else
                {
                    _TextOn = value;
                }
            }
        }
        #endregion

        #region Orthodyne Properties
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink6
        {
            get
            {
                return _VL[5];
            }
            set
            {
                _VL[5] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink7
        {
            get
            {
                return _VL[6];
            }
            set
            {
                _VL[6] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink8
        {
            get
            {
                return _VL[7];
            }
            set
            {
                _VL[7] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink9
        {
            get
            {
                return _VL[8];
            }
            set
            {
                _VL[8] = value;
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
        [Description("Indique quand le relais est accessible")]
        public ModeRelais RelaisMode
        {
            get
            {
                return _ModeRelais;
            }
            set
            {
                _ModeRelais = value;
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
                return "REL";
            }
        }

        public Type GType()
        {
            return GetType();
        }
    }
}

public enum ModeRelais
{
    Normal = 0,
    If_Not_Job_On_Run = 1,
    If_On_Manual_Mod = 2,
    Manual_Mod_And_Not_Job_On_Run = 3
}
