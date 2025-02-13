using Microsoft.VisualBasic;
using StageCode;
using StageCode.LIB;
using StageCode.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static StageCode.Other.Langs;

namespace OrthoDesigner.LIB
{
    public partial class OrthoTabname : UserControl
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
        private string[] _VL = new string[9];

        public CButton btn = new CButton();
        private string _OngletCible; // Page qui sera chargée lors du clic sur le bouton
        private string _captionValues = "OrthoTabName";
        private string _visibility = "1";

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;
        public OrthoTabname()
        {
            InitializeComponent();
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

        private void OrthoTabName_Resize(object sender, EventArgs e)
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
            this.comment = comment;

            var StyleText = new FontStyle();

            TextAlign = ContentAlignment_Parser.Get_Alignment(int.Parse(splitPvirgule[3]));

            // Ajouter des styles de texte conditionnellement
            if (splitPvirgule[9] == "True")
            {
                StyleText |= FontStyle.Strikeout; // Utiliser l'opérateur bitwise OR pour combiner les styles
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
            Precision = splitPvirgule[4];
            Caption = splitPvirgule[2];
            BackColor = FromOle(splitPvirgule[5]);
            ForeColor = FromOle(splitPvirgule[6]);
            if (Enum.TryParse<TDesign>(splitPvirgule[13], out TDesign typeDesignResult))
            {
                TypeDesign = typeDesignResult;  
            }
            else
            {
                TypeDesign = TDesign.Bouton;  
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
            OngletCible = splitPvirgule[19];
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

            if (splitPvirgule.Length >= 34)
            {
                Visibility = splitPvirgule[33];
            }
            return this;
        }

        public static OrthoTabname ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);

            OrthoTabname am60Control = new OrthoTabname();

            am60Control.Caption = xml.Attribute("name")?.Value ?? "";

            // Parse the <Apparence> section
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                am60Control.TextAlign = ContentAlignment_Parser.Get_Alignment(int.Parse(appearance.Element("TextAlign")?.Attribute("value")?.Value ?? "0"));
                am60Control.Precision = (appearance.Element("Precision")?.Attribute("value")?.Value ?? "0");
                am60Control.BackColor = ColorTranslator.FromOle(int.Parse(appearance.Element("Backcolor")?.Attribute("value")?.Value ?? "0"));
                am60Control.ForeColor = ColorTranslator.FromOle(int.Parse(appearance.Element("Forecolor")?.Attribute("value")?.Value ?? "0"));

                // Parse <Font> section
                XElement? fontElement = appearance.Element("Font");
                if (fontElement != null)
                {
                    am60Control.Font = new Font(
                        fontElement.Element("Name")?.Attribute("value")?.Value ?? "Arial",
                        float.Parse(fontElement.Element("Size")?.Attribute("value")?.Value ?? "12"),
                        (fontElement.Element("Bold")?.Attribute("value")?.Value == "true" ? FontStyle.Bold : 0) |
                        (fontElement.Element("Italic")?.Attribute("value")?.Value == "true" ? FontStyle.Italic : 0) |
                        (fontElement.Element("Underline")?.Attribute("value")?.Value == "true" ? FontStyle.Underline : 0) |
                        (fontElement.Element("Strikeout")?.Attribute("value")?.Value == "true" ? FontStyle.Strikeout : 0)
                    );
                }

                am60Control.TypeDesign = (TDesign)int.Parse(appearance.Element("TypeDesign")?.Attribute("value")?.Value ?? "0");
                am60Control.BorderWidth = int.Parse(appearance.Element("BorderWidth")?.Attribute("value")?.Value ?? "1");

                // Extract Size and Location
                am60Control.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "100")
                );
                am60Control.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")
                );

                am60Control.OngletCible = appearance.Element("OngletCible")?.Attribute("value")?.Value ?? "";

                // Extract <Values> section
                XElement? valuesElement = appearance.Element("Values");
                if (valuesElement != null)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        am60Control._VL[i] = valuesElement.Element($"VL{i}")?.Attribute("value")?.Value ?? "";
                    }
                }

                am60Control.ColorOn = ColorTranslator.FromOle(int.Parse(appearance.Element("ColorOn")?.Attribute("value")?.Value ?? "0"));
                am60Control.ColorOff = ColorTranslator.FromOle(int.Parse(appearance.Element("ColorOff")?.Attribute("value")?.Value ?? "0"));
                am60Control.ColorErr = ColorTranslator.FromOle(int.Parse(appearance.Element("ColorErr")?.Attribute("value")?.Value ?? "0"));
                am60Control.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Attribute("value")?.Value ?? "0");
                am60Control.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Attribute("value")?.Value ?? "0");
                am60Control.Visibility = appearance.Element("Visibility")?.Attribute("value")?.Value ?? "Visible";
            }

            return am60Control;
        }

        public string WriteFile()
        {
            string retour = "ORTHO;TABNAME;" + Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(TextAlign).ToString() + ";" + Precision + ";" + ToOle(BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + OngletCible + ";" + _VL[1] + ";" + _VL[2] + ";" + _VL[3] + ";" + _VL[4] + ";" + _VL[5] + ";" + _VL[6] + ";" + _VL[7] + ";" + _VL[8] + (";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility);

            return retour;
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            // Début du composant spécifique
            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{SecurityElement.Escape(this.Caption)}\">");

            // Section Apparence
            xmlContent.AppendLine("      <Apparence>");
            xmlContent.AppendLine($"        <TextAlign value=\"{ContentAlignment_Parser.Get_ValueToWrite(TextAlign)}\"/>");
            xmlContent.AppendLine($"        <Precision value=\"{Precision}\"/>");
            xmlContent.AppendLine($"        <Backcolor value=\"{ToOle(BackColor)}\"/>");
            xmlContent.AppendLine($"        <Forecolor value=\"{ToOle(ForeColor)}\"/>");

            // Section Police
            xmlContent.AppendLine("      <Font>");
            xmlContent.AppendLine($"        <Name value=\"{Font.Name}\"/>");
            xmlContent.AppendLine($"        <Size value=\"{Font.Size}\"/>");
            xmlContent.AppendLine($"        <Strikeout value=\"{Font.Strikeout.ToString().ToLower()}\"/>");
            xmlContent.AppendLine($"        <Underline value=\"{Font.Underline.ToString().ToLower()}\"/>");
            xmlContent.AppendLine($"        <Bold value=\"{Font.Bold.ToString().ToLower()}\"/>");
            xmlContent.AppendLine($"        <Italic value=\"{Font.Italic.ToString().ToLower()}\"/>");
            xmlContent.AppendLine("      </Font>");

            xmlContent.AppendLine($"        <TypeDesign value=\"{Convert.ToInt32(TypeDesign)}\"/>");
            xmlContent.AppendLine($"        <BorderWidth value=\"{BorderWidth}\"/>");

            // Ajout des informations de taille et de position
            xmlContent.AppendLine($"        <SizeWidth>{this.Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <SizeHeight>{this.Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <LocationX>{this.Location.X}</LocationX>");
            xmlContent.AppendLine($"        <LocationY>{this.Location.Y}</LocationY>");

            // Autres paramètres
            xmlContent.AppendLine($"        <OngletCible value=\"{OngletCible}\"/>");

            // Gestion des valeurs _VL
            xmlContent.AppendLine("      <Values>");
            for (int i = 1; i <= 8; i++)
            {
                xmlContent.AppendLine($"        <VL{i} value=\"{_VL[i]}\"/>");
            }
            xmlContent.AppendLine("      </Values>");

            // Couleurs et visibilité
            xmlContent.AppendLine($"        <ColorOn value=\"{ToOle(ColorOn)}\"/>");
            xmlContent.AppendLine($"        <ColorOff value=\"{ToOle(ColorOff)}\"/>");
            xmlContent.AppendLine($"        <ColorErr value=\"{ToOle(ColorErr)}\"/>");
            xmlContent.AppendLine($"        <LevelVisible value=\"{LevelVisible.ToString().ToLower()}\"/>");
            xmlContent.AppendLine($"        <LevelEnabled value=\"{LevelEnabled.ToString().ToLower()}\"/>");
            xmlContent.AppendLine($"        <Visibility value=\"{Visibility}\"/>");

            xmlContent.AppendLine("      </Apparence>");

            // Fermeture du composant
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
        [Description("Nom de l'onglet cible")]
        public string OngletCible
        {
            get
            {
                return _OngletCible;
            }
            set
            {
                _OngletCible = value;
            }
        }
        [Category("Apparence")]
        [ShowOnProtectedMode()]
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
        #endregion

        #region Orthodyne Properties
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
                return "TABNAME";
            }
        }

        public Type GType()
        {
            return GetType();
        }
        private void Tmp_Load(object sender, EventArgs e)
        {
            Debug.WriteLine(typeof(OrthoTabname).AssemblyQualifiedName);

            this.Controls.Add(btn);
            BackColor = Color.Transparent;
            btn.FillType = CButton.eFillType.Solid;
            btn.BackColor = Color.Transparent;

            Langs.LanguageChanged += LanguageChangedEventHandler;
            LanguageChangedEventHandler(Langs.CurrentLanguage);

            ControlUtils.RegisterControl(btn, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoTabName_Resize;
        }
    }
}
