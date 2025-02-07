using Microsoft.VisualBasic;
using StageCode.Other;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using static StageCode.Other.Langs;

namespace StageCode.LIB
{
    public enum TDesign
    {
        Bouton = 1,
        Rectangle,
        Cercle,
        Arrondi,
        Tedit,
        Combo
    }
    public partial class OrthoAD : UserControl
    {
        private string _captionValues = "OrthoAD";
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        private TDesign _TypeDesign = TDesign.Arrondi; // Type d'entrée du champ (cf. Classe TDesign)
        private string _Format; // Format de la variable (précision)
        private string _comment = ""; // Commentaire sur le contrôle
        private int _BorderW = 0;

        private CButton btn = new CButton();
        private string _VarText; // Varlink 1
        private string _VarForeColor; // Varlink 3
        private string _VarBackColor; // Varlink 2
        private string _VarValMax; // Varlink 4 : Reference Max
        private string _VarTextMax; // Varlink 5 : Texte affiche si plus haut
        private string _VarValMin; // Varlink 6 : Reference Min
        private string _VarTextMin; // Varlink 7 : Texte affiche si plus petit que valmin
        private string _visibility = "1";
        /// <summary>
        /// Param non utilisé mais visible au cas où
        /// </summary>
        private string[] _VL = new string[9];

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        private void OrthoAD_Load(object sender, EventArgs e)
        {

        }

        public OrthoAD()
        {
            InitializeComponent();

            Langs.LanguageChanged += LanguageChangedEventHandler;

            this.Controls.Add(btn);
            btn.Text = this.Name;
            BackColor = Color.Transparent;
            btn.FillType = CButton.eFillType.Solid;
            btn.Shape = CButton.eShape.Rectangle;
            btn.Corners.All = 6;

            ControlUtils.RegisterControl(btn, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoResult_Resize;
            base.Load += OrthoAD_Load;
        }
        private void OrthoResult_Resize(object sender, EventArgs e)
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
        public static OrthoAD ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);
            OrthoAD OrthoADControl = new OrthoAD();

            // Récupérer l'attribut 'type' et 'name' du composant
            string? type = xml.Attribute("type")?.Value;
            OrthoADControl.Name = xml.Attribute("name")?.Value;

            // Parse the <Apparence> section
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                // Récupérer tous les attributs de la section Apparence
                OrthoADControl.Caption = appearance.Element("Caption")?.Value ?? string.Empty;
                OrthoADControl.TextAlign = ContentAlignment_Parser.Get_Alignment(int.Parse(appearance.Element("TextAlign")?.Value ?? "0"));
                OrthoADControl.Format = appearance.Element("Format")?.Value ?? string.Empty;

                // Récupérer les couleurs
                OrthoADControl.BackColor = FromOlInve(appearance.Element("BackColor")?.Value);
                OrthoADControl.ForeColor = FromOlInve(appearance.Element("ForeColor")?.Value);

                // Récupérer les propriétés de la police
                string fontName = appearance.Element("FontName")?.Value ?? "Arial";
                float fontSize = float.Parse(appearance.Element("FontSize")?.Value ?? "12");
                FontStyle fontStyle = FontStyle.Regular;

                if (appearance.Element("FontStrikeout")?.Value == "True") fontStyle |= FontStyle.Strikeout;
                if (appearance.Element("FontUnderline")?.Value == "True") fontStyle |= FontStyle.Underline;
                if (appearance.Element("FontBold")?.Value == "True") fontStyle |= FontStyle.Bold;
                if (appearance.Element("FontItalic")?.Value == "True") fontStyle |= FontStyle.Italic;

                OrthoADControl.Font = new Font(fontName, fontSize, fontStyle);

                // Récupérer les informations de type de design
                var typeDesignValue = appearance.Element("TypeDesign")?.Value;
                if (Enum.IsDefined(typeof(TDesign), typeDesignValue))
                {
                    OrthoADControl.TypeDesign = (TDesign)Enum.Parse(typeof(TDesign), typeDesignValue);
                }

                // Récupérer les dimensions et position
                OrthoADControl.BorderWidth = int.Parse(appearance.Element("BorderWidth")?.Value ?? "0");
                OrthoADControl.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "100")
                );
                OrthoADControl.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")
                );

                // Variables additionnelles
                OrthoADControl.VarText = appearance.Element("VarText")?.Value ?? string.Empty;
                OrthoADControl.VarBackColor = appearance.Element("VarBackColor")?.Value ?? string.Empty;
                OrthoADControl.VarForeColor = appearance.Element("VarForeColor")?.Value ?? string.Empty;
                OrthoADControl.VarValMax = appearance.Element("VarValMax")?.Value ?? string.Empty;
                OrthoADControl.VarTextMax = appearance.Element("VarTextMax")?.Value ?? string.Empty;
                OrthoADControl.VarValMin = appearance.Element("VarValMin")?.Value ?? string.Empty;
                OrthoADControl.VarTextMin = appearance.Element("VarTextMin")?.Value ?? string.Empty;

                // Récupérer les valeurs VL7 et VL8
                OrthoADControl._VL[7] = appearance.Element("VL7")?.Value ?? string.Empty;
                OrthoADControl._VL[8] = appearance.Element("VL8")?.Value ?? string.Empty;

                // Récupérer les couleurs d'état
                OrthoADControl.ColorOn = FromOlInve(appearance.Element("ColorOn")?.Value);
                OrthoADControl.ColorOff = FromOlInve(appearance.Element("ColorOff")?.Value);
                OrthoADControl.ColorErr = FromOlInve(appearance.Element("ColorErr")?.Value);

                // Récupérer les niveaux de visibilité et d'activation
                OrthoADControl.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Value ?? "0");
                OrthoADControl.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Value ?? "0");

                // Récupérer la visibilité
                OrthoADControl.Visibility = appearance.Element("Visibility")?.Value ?? "Visible";
            }

            return OrthoADControl;
        }

        private static Color FromOlInve(string oleColor)
        {
            // Implémentation pour convertir une couleur Ole en couleur .NET
            if (string.IsNullOrEmpty(oleColor)) return Color.Transparent;
            return ColorTranslator.FromHtml(oleColor);  // On suppose que les couleurs sont au format hexadécimal
        }


        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            // Début du composant spécifique
            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");

            // Section Apparence
            xmlContent.AppendLine("      <Apparence>");
            xmlContent.AppendLine($"        <Caption>{Caption}</Caption>");
            xmlContent.AppendLine($"        <TextAlign>{ContentAlignment_Parser.Get_ValueToWrite(this.TextAlign)}</TextAlign>");
            xmlContent.AppendLine($"        <Format>{Format}</Format>");
            xmlContent.AppendLine($"        <BackColor>{ToOle(this.BackColor)}</BackColor>");
            xmlContent.AppendLine($"        <ForeColor>{ToOle(this.ForeColor)}</ForeColor>");
            xmlContent.AppendLine($"        <FontName>{this.Font.Name}</FontName>");
            xmlContent.AppendLine($"        <FontSize>{this.Font.Size}</FontSize>");
            xmlContent.AppendLine($"        <FontStrikeout>{this.Font.Strikeout}</FontStrikeout>");
            xmlContent.AppendLine($"        <FontUnderline>{this.Font.Underline}</FontUnderline>");
            xmlContent.AppendLine($"        <FontBold>{this.Font.Bold}</FontBold>");
            xmlContent.AppendLine($"        <FontItalic>{this.Font.Italic}</FontItalic>");
            xmlContent.AppendLine($"        <TypeDesign>{Convert.ToInt32(this.TypeDesign)}</TypeDesign>");
            xmlContent.AppendLine($"        <BorderWidth>{this.BorderWidth}</BorderWidth>");
            xmlContent.AppendLine($"        <SizeHeight>{this.Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <SizeWidth>{this.Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <LocationY>{this.Location.Y}</LocationY>");
            xmlContent.AppendLine($"        <LocationX>{this.Location.X}</LocationX>");
            xmlContent.AppendLine($"        <VarText>{this.VarText}</VarText>");
            xmlContent.AppendLine($"        <VarBackColor>{this.VarBackColor}</VarBackColor>");
            xmlContent.AppendLine($"        <VarForeColor>{this.VarForeColor}</VarForeColor>");
            xmlContent.AppendLine($"        <VarValMax>{this.VarValMax}</VarValMax>");
            xmlContent.AppendLine($"        <VarTextMax>{this.VarTextMax}</VarTextMax>");
            xmlContent.AppendLine($"        <VarValMin>{this.VarValMin}</VarValMin>");
            xmlContent.AppendLine($"        <VarTextMin>{this.VarTextMin}</VarTextMin>");
            xmlContent.AppendLine($"        <VL7>{this._VL[7]}</VL7>");
            xmlContent.AppendLine($"        <VL8>{this._VL[8]}</VL8>");
            xmlContent.AppendLine($"        <ColorOn>{ToOle(this.ColorOn)}</ColorOn>");
            xmlContent.AppendLine($"        <ColorOff>{ToOle(this.ColorOff)}</ColorOff>");
            xmlContent.AppendLine($"        <ColorErr>{ToOle(this.ColorErr)}</ColorErr>");
            xmlContent.AppendLine($"        <LevelVisible>{this.LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"        <LevelEnabled>{this.LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"        <Visibility>{this.Visibility}</Visibility>");
            xmlContent.AppendLine("      </Apparence>");

            // Fermeture du composant
            xmlContent.AppendLine("    </Component>");

            // Retourner le contenu XML généré
            return xmlContent.ToString();
        }

        public string WriteFile()
        {
            return "ORTHO;AD;" + Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(TextAlign).ToString() + ";" + Format + ";" + ToOle(BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + VarText + ";" + VarBackColor + ";" + VarForeColor + ";" + VarValMax + ";" + VarTextMax + ";" + VarValMin + ";" + VarTextMin + ";" + _VL[7] + ";" + _VL[8] + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;
        }


        #endregion

        #region Label Properties
        [Category("Apparence")]
        [Description("Détermine la potition du texte dans ce contrôle")]
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
        [ShowOnProtectedMode()]
        [Description("La couleur d'arrière plan du contrôle.")]
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
        [Description("La couleur d'arrière plan du contrôle.")]
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
        [Category("Apparence")]
        [Description("Texte du label")]
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
                return btn.BackColor;
            }
            set
            {
                btn.BackColor = value;
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
        [Category("Affichage conditionnel")]
        [ShowOnProtectedMode()]
        [Description("Var ou valeur qui définit la valeur maximum.")]
        public string VarValMax
        {
            get
            {
                return _VarValMax;
            }
            set
            {
                _VarValMax = value;
            }
        }
        [Category("Affichage conditionnel")]
        [ShowOnProtectedMode()]
        [Description("Texte/Var affiché si la valeur est plus grande ou égale que la valeur de référence définie dans VarValMax")]
        public string VarTextMax
        {
            get
            {
                return _VarTextMax;
            }
            set
            {
                _VarTextMax = value;
            }
        }
        [Category("Affichage conditionnel")]
        [ShowOnProtectedMode()]
        [Description("Texte/Var affiché si la valeur est plus petite que la valeur de référence définie dans VarValMin")]
        public string VarTextMin
        {
            get
            {
                return _VarTextMin;
            }
            set
            {
                _VarTextMin = value;
            }
        }
        [Category("Affichage conditionnel")]
        [ShowOnProtectedMode()]
        [Description("Var ou valeur qui définit la valeur minimum.")]
        public string VarValMin
        {
            get
            {
                return _VarValMin;
            }
            set
            {
                _VarValMin = value;
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
        [Category("Texte")]
        [Description("Nom de la variable qui donne le texte.")]
        public string VarText
        {
            get
            {
                return _VarText;
            }
            set
            {
                _VarText = value;
            }
        }
        [Category("Texte")]
        [Description("Nom de la variable qui donne la couleur d'arrière plan.")]
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
        [Category("Texte")]
        [Description("Nom de la variable qui donne la couleur du texte.")]
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
                return base.AccessibleDescription;
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
                return base.AccessibleName;
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
                return base.BackgroundImage;
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
                return base.ContextMenuStrip;
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
                return base.Tag;
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
                return "AD";
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
                int idx = Convert.ToInt32(NewLanguage.LanguageID);
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

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ShowOnProtectedModeAttribute : Attribute
    {
    }
}


