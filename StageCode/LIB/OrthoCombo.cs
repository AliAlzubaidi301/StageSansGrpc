using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static StageCode.LIB.OrthoAD;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace StageCode.LIB
{
    public partial class OrthoCombo : UserControl
    {
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        private TDesign _TypeDesign; // Type d'entrée du champ (cf. Classe TDesign)
        private string _Format; // Format de la variable (précision)
        private string _comment = ""; // Commentaire sur le contrôle
        private int _BorderW = 0;

        private string _Variable; // Nom de la variable
        private string _Values; // Valeurs de la combobox
        private HorizontalAlignment _TextAlign;
        public System.Windows.Forms.ComboBox ComboBox;
        private string _visibility = "1";
        /// <summary>
        /// Param non utilisé mais visible au cas où
        /// </summary>
        private string[] _VL = new string[9];
        private string Det;

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoCombo()
        {
            InitializeComponent();
            System.Windows.Forms.ComboBox ComboBox = ComboBox1;
            AutoSize = false;
            ComboBox1.FlatStyle = FlatStyle.Flat;
            ComboBox1.FormattingEnabled = true;
            ControlUtils.RegisterControl(ComboBox1, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoCombo_Resize;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int CB_ERR = -1;
        private const int CB_SETITEMHEIGHT = 0x153;
        private bool SetComboBoxEditHeight(System.Windows.Forms.ComboBox ComboBox, int Height)
        {
            return SendMessage(ComboBox.Handle, CB_SETITEMHEIGHT, new IntPtr(-1), new IntPtr(Height)) != new IntPtr(CB_ERR);
            //ComboBox.Refresh();
        }
        private void OrthoCombo_Resize(object sender, EventArgs e)
        {
            ComboBox1.Size = this.Size;
            SetComboBoxEditHeight(ComboBox1, this.Height - 6);
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
            var StyleText = FontStyle.Regular;

            if (splitPvirgule[9] == "True")
            {
                StyleText |= FontStyle.Strikeout;
            }
            if (splitPvirgule[10] == "True")
            {
                StyleText |= FontStyle.Underline;
            }
            if (splitPvirgule[11] == "True")
            {
                StyleText |= FontStyle.Bold;
            }
            if (splitPvirgule[12] == "True")
            {
                StyleText |= FontStyle.Italic;
            }

            this.Name = splitPvirgule[1] + "_" + splitPvirgule[2];
            Format = splitPvirgule[4];
            Font = new Font(splitPvirgule[7], int.Parse(splitPvirgule[8]), StyleText);
            this.Text = splitPvirgule[2];
            TextAlign = (HorizontalAlignment)int.Parse(splitPvirgule[3]);
            try
            {
                BackColor = FromOle(splitPvirgule[5]);
            }
            catch
            {
            }
            try
            {
                ForeColor = FromOle(splitPvirgule[6]);
            }
            catch
            {
            }
            TypeDesign = (TDesign)int.Parse(splitPvirgule[13]);
            BorderWidth = int.Parse(splitPvirgule[14]);
            this.Size = new Size(int.Parse(splitPvirgule[16]), int.Parse(splitPvirgule[15]));
            this.Location = new Point(int.Parse(splitPvirgule[18]), int.Parse(splitPvirgule[17]));
            ColorOn = FromOle(splitPvirgule[28]);
            ColorOff = FromOle(splitPvirgule[29]);
            ColorErr = FromOle(splitPvirgule[30]);
            LevelVisible = int.Parse(splitPvirgule[31]);
            LevelEnabled = int.Parse(splitPvirgule[32]);
            this.comment = comment;
            if (FromCopy)
            {
                this.Location = new Point((int)Math.Round(Double.Parse(splitPvirgule[18]) + 10d), (int)Math.Round(Double.Parse(splitPvirgule[17]) + 10d));
            }
            else
            {
                this.Location = new Point(int.Parse(splitPvirgule[18]), int.Parse(splitPvirgule[17]));
            }
            this.Size = new Size(int.Parse(splitPvirgule[16]), int.Parse(splitPvirgule[15]));

            Variable = splitPvirgule[19];
            Value = splitPvirgule[20];
            _VL[2] = splitPvirgule[21];
            _VL[3] = splitPvirgule[22];
            _VL[4] = splitPvirgule[23];
            _VL[5] = splitPvirgule[24];
            _VL[6] = splitPvirgule[25];
            _VL[7] = splitPvirgule[26];
            Det = splitPvirgule[27];

            if (splitPvirgule.Length >= 34)
            {
                Visibility = splitPvirgule[33];
            }
            return this;
        }

        public string WriteFile()
        {
            return "ORTHO;COMBO;" + Text + ";" + TextAlign.ToString() + ";" + Format + ";" + ToOle(BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + Variable + ";" + Value + ";" + _VL[2] + ";" + _VL[3] + ";" + _VL[4] + ";" + _VL[5] + ";" + _VL[6] + ";" + _VL[7] + ";" + Det + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;
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
            xmlContent.AppendLine($"        <TypeDesign>{Convert.ToInt32(TypeDesign)}</TypeDesign>");
            xmlContent.AppendLine($"        <BorderWidth>{BorderWidth}</BorderWidth>");
            xmlContent.AppendLine($"        <SizeHeight>{Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <SizeWidth>{Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <LocationY>{Location.Y}</LocationY>");
            xmlContent.AppendLine($"        <LocationX>{Location.X}</LocationX>");
            xmlContent.AppendLine("      </Apparence>");

            xmlContent.AppendLine($"      <Variable>{Variable}</Variable>");
            xmlContent.AppendLine($"      <Value>{Value}</Value>");

            // Variables _VL
            for (int i = 2; i < _VL.Length; i++)
            {
                xmlContent.AppendLine($"      <VL{i}>{_VL[i]}</VL{i}>");
            }

            xmlContent.AppendLine($"      <Det>{Det}</Det>");
            xmlContent.AppendLine($"      <ColorOn>{ToOle(ColorOn)}</ColorOn>");
            xmlContent.AppendLine($"      <ColorOff>{ToOle(ColorOff)}</ColorOff>");
            xmlContent.AppendLine($"      <ColorErr>{ToOle(ColorErr)}</ColorErr>");
            xmlContent.AppendLine($"      <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"      <LevelEnabled>{LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"      <Visibility>{Visibility}</Visibility>");

            xmlContent.AppendLine("    </Component>");

            return xmlContent.ToString();
        }

        #endregion

        #region ComboBox Properties
        [Category("Apparence")]
        [Description("Indique la façon dont le texte doit être aligné dans le contrôle d'édition.")]
        public HorizontalAlignment TextAlign
        {
            get
            {
                return _TextAlign;
            }
            set
            {
                _TextAlign = value;
            }
        }
        [Category("Apparence")]
        [Description("La couleur d'arrière plan du contrôle.")]
        public Color BackColor
        {
            get
            {
                return ComboBox1.BackColor;
            }
            set
            {
                ComboBox1.BackColor = value;
            }
        }
        [Category("Apparence")]
        [ShowOnProtectedMode()]
        [Description("La couleur d'arrière plan du contrôle.")]
        public Font Font
        {
            get
            {
                return ComboBox1.Font;
            }
            set
            {
                ComboBox1.Font = value;
            }
        }
        [Category("Apparence")]
        [Description("La couleur d'arrière plan du contrôle.")]
        public Color ForeColor
        {
            get
            {
                return ComboBox1.ForeColor;
            }
            set
            {
                ComboBox1.ForeColor = value;
            }
        }
        #endregion

        #region Control properties
        [Category("Orthodyne")]
        [Description("Nom de la variable")]
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
        [Description("Les valeurs de la combo séparées par des '|'")]
        public string Value
        {
            get
            {
                return _Values;
            }
            set
            {
                _Values = value;
            }
        }

        [Category("Orthodyne")]
        [Description("Indiquer IDX pour retourner l'index de la sélection dans la variable au lieu du texte")]
        public string ReturnIndex
        {
            get
            {
                return _VL[2];
            }
            set
            {
                _VL[2] = value;
            }
        }
        #endregion

        #region Orthodyne Properties
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink3
        {
            get
            {
                return _VL[2];
            }
            set
            {
                _VL[2] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink4
        {
            get
            {
                return _VL[3];
            }
            set
            {
                _VL[3] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink5
        {
            get
            {
                return _VL[4];
            }
            set
            {
                _VL[4] = value;
            }
        }
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
                return "COMBO";
            }
        }

        public Type GType()
        {
            return GetType();
        }

        private void OrthoCombo_Load(object sender, EventArgs e)
        {

        }

        private void ComboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
