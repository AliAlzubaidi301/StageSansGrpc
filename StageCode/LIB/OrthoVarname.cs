using Microsoft.VisualBasic;
using StageCode.LIB;
using StageCode.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static StageCode.LIB.OrthoAD;

namespace StageCode.LIB
{
    public partial class OrthoVarname : UserControl
    {
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        private TDesign _TypeDesign = TDesign.Rectangle; // LBO Force à Rectangle Type d'entrée du champ (cf. Classe TDesign)

        private string _Format; // Format de la variable (précision)
        private string _comment = ""; // Commentaire sur le contrôle
        private int _BorderW = 0;

        public System.Windows.Forms.Label Label;
        private string _VarText; // Nom de la variable qui donne le texte a afficher
        private string _VarBackColor; // Nom de la variable qui donne la couleur de fond
        private string _VarForeColor; // Nom de la variable qui donne la couleur du texte
        private string _VarTest; // nom de la Variable qui test si il y a un maximum
        private string _VarInfo; // Nom de la Variable qui donne le texte d'info
        private string _VarInfoBackColor; // Nom de la variable qui donne la couleur de fond du texte d'info
        private string _VarInfoForeColor; // Nom de la variable qui donne la couleur du texte d'info
        /// <summary>
        /// Param non utilisé mais visible au cas où
        /// </summary>
        private string[] _VL = new string[9];
        private string _visibility = "";

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoVarname()
        {
            InitializeComponent();
            Label = Label1;
            // Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            AutoSize = false;
            Label1.AutoSize = false;
            Label1.Text = this.Name;
            ControlUtils.RegisterControl(Label1, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoResult_Resize;
        }

        private void OrthoVarname_Load(object sender, EventArgs e)
        {

        }

        private void OrthoResult_Resize(object sender, EventArgs e)
        {
            Label1.Size = this.Size;
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
            Format = splitPvirgule[4];
            Font = new Font(splitPvirgule[7], float.Parse(splitPvirgule[8]), StyleText);
            this.Text = splitPvirgule[2];
            this.BackColor = FromOle(splitPvirgule[5]);
            ForeColor = FromOle(splitPvirgule[6]);
            if (Enum.TryParse(splitPvirgule[13], true, out TDesign result))
            {
                TypeDesign = result;
            }
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
            VarText = splitPvirgule[19];
            VarBackColor = splitPvirgule[20];
            VarForeColor = splitPvirgule[21];
            VarTest = splitPvirgule[22];
            VarInfo = splitPvirgule[23];
            VarInfoBackColor = splitPvirgule[24];
            VarInfoForeColor = splitPvirgule[25];
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
            string retour = "ORTHO;VARNAME;" + this.Text + ";" + ContentAlignment_Parser.Get_ValueToWrite(TextAlign).ToString() + ";" + Format + ";" + ToOle(this.BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + VarText + ";" + VarBackColor + ";" + VarForeColor + ";" + VarTest + ";" + VarInfo + ";" + VarInfoBackColor + ";" + VarInfoForeColor + ";" + _VL[7] + ";" + _VL[8] + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;

            return retour;
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"<Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("  <Apparence>");

            // Properties
            xmlContent.AppendLine($"    <Text>{Text}</Text>");
            xmlContent.AppendLine($"    <TextAlign>{ContentAlignment_Parser.Get_ValueToWrite(TextAlign)}</TextAlign>");
            xmlContent.AppendLine($"    <Format>{Format}</Format>");
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
            xmlContent.AppendLine($"    <VarText>{VarText}</VarText>");
            xmlContent.AppendLine($"    <VarBackColor>{VarBackColor}</VarBackColor>");
            xmlContent.AppendLine($"    <VarForeColor>{VarForeColor}</VarForeColor>");
            xmlContent.AppendLine($"    <VarTest>{VarTest}</VarTest>");
            xmlContent.AppendLine($"    <VarInfo>{VarInfo}</VarInfo>");
            xmlContent.AppendLine($"    <VarInfoBackColor>{VarInfoBackColor}</VarInfoBackColor>");
            xmlContent.AppendLine($"    <VarInfoForeColor>{VarInfoForeColor}</VarInfoForeColor>");

            // _VL values
            for (int i = 7; i < _VL.Length; i++)
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

        #region Label Properties
        [Category("Apparence")]
        [Description("Détermine la potition du texte dans ce contrôle")]
        public ContentAlignment TextAlign
        {
            get
            {
                return Label1.TextAlign;
            }
            set
            {
                Label1.TextAlign = value;
            }
        }
        [Category("Apparence")]
        [ShowOnProtectedMode()]
        [Description("La couleur d'arrière plan du contrôle.")]
        public Font Font
        {
            get
            {
                return Label1.Font;
            }
            set
            {
                Label1.Font = value;
            }
        }
        [Category("Apparence")]
        [Description("La couleur d'arrière plan du contrôle.")]
        public Color ForeColor
        {
            get
            {
                return Label1.ForeColor;
            }
            set
            {
                Label1.ForeColor = value;
            }
        }
        #endregion

        #region Varname Properties
        [Category("Orthodyne")]
        [Description("Variable qui donne le texte à afficher")]
        public string VarText
        {
            get
            {
                return _VarText;
            }
            set
            {
                _VarText = value;
                Label1.Text = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Variable qui donne la couleur de fond")]
        public string VarBackColor
        {
            get
            {
                return _VarBackColor;
            }
            set
            {
                _VarBackColor = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Variable qui donne la couleur du texte")]
        public string VarForeColor
        {
            get
            {
                return _VarForeColor;
            }
            set
            {
                _VarForeColor = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Test si il y a une valeur maximum spécifiée")]
        public string VarTest
        {
            get
            {
                return _VarTest;
            }
            set
            {
                _VarTest = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Variable qui donne le texte d'info supplémentaire")]
        public string VarInfo
        {
            get
            {
                return _VarInfo;
            }
            set
            {
                _VarInfo = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Variable qui donne la couleur de fond pour l'info")]
        public string VarInfoBackColor
        {
            get
            {
                return _VarInfoBackColor;
            }
            set
            {
                _VarInfoBackColor = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Variable qui donne la couleur du texte pour l'info")]
        public string VarInfoForeColor
        {
            get
            {
                return _VarInfoForeColor;
            }
            set
            {
                _VarInfoForeColor = value;
            }
        }
        #endregion

        #region Orthodyne Properties
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
        [DefaultValue("")]
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
                if (string.IsNullOrEmpty(value))
                    value = "1";
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
                return AccessibleRole;
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
                return Visible;
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
                return Anchor;
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
                return AutoScroll;
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
                return AutoScrollMargin;
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
                return "VARNAME";
            }
        }

        public Type GType()
        {
            return GetType();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}