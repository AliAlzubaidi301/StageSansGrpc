using Microsoft.VisualBasic;
using StageCode.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static StageCode.LIB.OrthoAD;

namespace StageCode.LIB
{
    public partial class OrthoDI : UserControl
    {
        public OrthoDI()
        {
            InitializeComponent();
            this.Controls.Add(btn);
            btn.Text = this.Name;
            BackColor = Color.Transparent;
            btn.FillType = CButton.eFillType.Solid;
            btn.Shape = CButton.eShape.Ellipse;
            // Ajoutez une initialisation quelconque après l'appel InitializeComponent().

            Langs.LanguageChanged += LanguageChangedEventHandler;

            ControlUtils.RegisterControl(btn, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoDI_Resize;
        }
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        private TDesign _TypeDesign = TDesign.Cercle; // Type d'entrée du champ (cf. Classe TDesign)
        private string _Format; // Format de la variable (précision)
        private string _comment = ""; // Commentaire sur le contrôle
        private int _BorderW;
        private string _visibility = "1";
        /// <summary>
        /// Param non utilisé mais visible au cas où
        /// </summary>
        private string[] _VL = new string[9];

        private string _captionValues = "OrthoDI";
        private CButton btn = new CButton();

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;


        // Déclaration des champs privés
        private string _simpleName = "Az";
        private string _flage="A";
        private String _ioStream="A";

        #region "Accesseurs"

        // Propriété pour SimpleName
        public string SimpleName
        {
            get { return _simpleName; }
            set { _simpleName = value; }
        }

        // Propriété pour Flage
        public string Flage
        {
            get { return _flage; }
            set { _flage = value; }
        }

        // Propriété pour IoStream
        public String IoStream
        {
            get { return _ioStream; }
            set { _ioStream = value; }
        }

        #endregion
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

        private void OrthoDI_Resize(object sender, EventArgs e)
        {
            if (btn.Shape == CButton.eShape.Ellipse)
            {
                btn.Size = new Size(this.Height, this.Height);
                this.Size = new Size(this.Height, this.Height);
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
            var StyleText = FontStyle.Regular;

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
            Precision = splitPvirgule[4];
            Caption = splitPvirgule[2];
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

            _VL[0] = splitPvirgule[19];
            _VL[1] = splitPvirgule[20];
            _VL[2] = splitPvirgule[21];
            _VL[3] = splitPvirgule[22];
            _VL[4] = splitPvirgule[23];
            _VL[5] = splitPvirgule[24];
            _VL[6] = splitPvirgule[25];
            _VL[7] = splitPvirgule[26];
            _VL[8] = splitPvirgule[27];

            ColorOn = FromOle(splitPvirgule[28]);
            ColorOff = FromOle(splitPvirgule[29]);
            ColorErr = FromOle(splitPvirgule[30]);
            LevelVisible = int.Parse(splitPvirgule[31]);
            LevelEnabled = int.Parse(splitPvirgule[32]);
            this.comment = comment;

            if (splitPvirgule.Length >= 33)
            {
                Visibility = splitPvirgule[33];
            }

            if (splitPvirgule.Length >= 34)
            {
                SimpleName = splitPvirgule[34];
                Flage = splitPvirgule[35];  // Adjust index based on position in the array
                IoStream = splitPvirgule[36];  // Adjust index based on position
            }
            else
            {
                SimpleName = "";
                Flage = "";
                IoStream = "";
            }

                return this;
        }

        public static OrthoDI ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);

            OrthoDI orthoCombo = new OrthoDI
            {
                Name = xml.Attribute("name")?.Value
            };

            // Parse the <Apparence> section
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                orthoCombo.Caption = appearance.Element("Caption")?.Value ?? "";
                orthoCombo.TextAlign = ContentAlignment_Parser.Get_Alignment(int.Parse(appearance.Element("TextAlign")?.Value ?? "0"));
                orthoCombo.Precision = (appearance.Element("Precision")?.Value ?? "0");
                orthoCombo.BackColor = System.Drawing.ColorTranslator.FromOle(int.Parse(appearance.Element("BackColor")?.Value ?? "0"));
                orthoCombo.ForeColor = System.Drawing.ColorTranslator.FromOle(int.Parse(appearance.Element("ForeColor")?.Value ?? "0"));
                orthoCombo.Font = new System.Drawing.Font(
                    appearance.Element("FontName")?.Value ?? "Arial",
                    float.Parse(appearance.Element("FontSize")?.Value ?? "10"),
                    (bool.Parse(appearance.Element("FontBold")?.Value ?? "False") ? System.Drawing.FontStyle.Bold : 0) |
                    (bool.Parse(appearance.Element("FontItalic")?.Value ?? "False") ? System.Drawing.FontStyle.Italic : 0) |
                    (bool.Parse(appearance.Element("FontUnderline")?.Value ?? "False") ? System.Drawing.FontStyle.Underline : 0) |
                    (bool.Parse(appearance.Element("FontStrikeout")?.Value ?? "False") ? System.Drawing.FontStyle.Strikeout : 0)
                );
                orthoCombo.TypeDesign = (TDesign)Enum.ToObject(typeof(TDesign), int.Parse(appearance.Element("TypeDesign")?.Value ?? "0"));
                orthoCombo.BorderWidth = int.Parse(appearance.Element("BorderWidth")?.Value ?? "0");
                orthoCombo.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "100")
                );
                orthoCombo.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")
                );
            }

            // Read Variables _VL
            List<string> vlList = new List<string>();
            for (int i = 0; ; i++)
            {
                XElement? vlElement = xml.Element($"VL{i}");
                if (vlElement == null) break;
                vlList.Add(vlElement.Value);
            }
            orthoCombo._VL = vlList.ToArray();

            orthoCombo.ColorOn = System.Drawing.ColorTranslator.FromOle(int.Parse(xml.Element("ColorOn")?.Value ?? "0"));
            orthoCombo.ColorOff = System.Drawing.ColorTranslator.FromOle(int.Parse(xml.Element("ColorOff")?.Value ?? "0"));
            orthoCombo.ColorErr = System.Drawing.ColorTranslator.FromOle(int.Parse(xml.Element("ColorErr")?.Value ?? "0"));
            orthoCombo.LevelVisible = int.Parse(xml.Element("LevelVisible")?.Value ?? "0");
            orthoCombo.LevelEnabled = int.Parse(xml.Element("LevelEnabled")?.Value ?? "0");
            orthoCombo.Visibility = xml.Element("Visibility")?.Value ?? "Visible";

            // Read SimpleName, Flag, and IoStream
            orthoCombo.SimpleName = xml.Element("SimpleName")?.Value ?? "";
            orthoCombo.Flage = (xml.Element("Flag")?.Value ?? string.Empty);
            orthoCombo.IoStream = xml.Element("IOStream")?.Value ?? "";

            return orthoCombo;
        }

        public string WriteFile()
        {
            return "ORTHO;DI;" + Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(TextAlign).ToString() + ";" + Precision + ";" + ToOle(BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + _VL[0] + ";" + _VL[1] + ";" + _VL[2] + ";" + _VL[3] + ";" + _VL[4] + ";" + _VL[5] + ";" + _VL[6] + ";" + _VL[7] + ";" + _VL[8] + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility + ";" + SimpleName + ";" + Flage.ToString() + ";" + IoStream;
        }

        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("      <Apparence>");
            xmlContent.AppendLine($"        <Caption>{Caption}</Caption>");
            xmlContent.AppendLine($"        <TextAlign>{ContentAlignment_Parser.Get_ValueToWrite(TextAlign)}</TextAlign>");
            xmlContent.AppendLine($"        <Precision>{Precision}</Precision>");
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

            // Variables _VL
            for (int i = 0; i < _VL.Length; i++)
            {
                xmlContent.AppendLine($"      <VL{i}>{_VL[i]}</VL{i}>");
            }

            xmlContent.AppendLine($"      <ColorOn>{ToOle(ColorOn)}</ColorOn>");
            xmlContent.AppendLine($"      <ColorOff>{ToOle(ColorOff)}</ColorOff>");
            xmlContent.AppendLine($"      <ColorErr>{ToOle(ColorErr)}</ColorErr>");
            xmlContent.AppendLine($"      <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"      <LevelEnabled>{LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"      <Visibility>{Visibility}</Visibility>");

            // Add SimpleName, Flag, and IOStream
            xmlContent.AppendLine($"      <SimpleName>{SimpleName}</SimpleName>");
            xmlContent.AppendLine($"      <Flag>{Flage}</Flag>");
            xmlContent.AppendLine($"      <IOStream>{IoStream}</IOStream>");

            xmlContent.AppendLine("    </Component>");

            return xmlContent.ToString();
        }
       
        #endregion

        #region Control Properties
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
        [Category("Apparence")]
        [Description("Caption du bouton")]
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
        public Color BackColor
        {
            get
            {
                return btn.BackColor;
            }
            set
            {
                btn.BackColor = value;
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

        #region Orthodyne Properties
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink1
        {
            get
            {
                return _VL[0];
            }
            set
            {
                _VL[0] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink2
        {
            get
            {
                return _VL[1];
            }
            set
            {
                _VL[1] = value;
            }
        }
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
                switch (value)
                {
                    case var @case when @case == TDesign.Bouton:
                        {
                            btn.Shape = CButton.eShape.Rectangle;
                            btn.Corners.All = 0;
                            break;
                        }
                    case var case1 when case1 == TDesign.Arrondi:
                        {
                            btn.Shape = CButton.eShape.Rectangle;
                            btn.Corners.All = 6;
                            break;
                        }
                    case var case2 when case2 == TDesign.Rectangle:
                        {
                            btn.Shape = CButton.eShape.Rectangle;
                            btn.Corners.All = 0;
                            break;
                        }
                    case var case3 when case3 == TDesign.Cercle:
                        {
                            btn.Shape = CButton.eShape.Ellipse;
                            btn.Size = new Size(this.Height, this.Height);
                            this.Size = btn.Size;
                            break;
                        }
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
        public new BorderStyle BorderStyle
        {
            get => base.BorderStyle;
            set => base.BorderStyle = value;
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
                return "DI";
            }
        }

        public Type GType()
        {
            return GetType();
        }

        private void OrthoDI_Load_1(object sender, EventArgs e)
        {

        }
    }
}
