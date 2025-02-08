using Microsoft.VisualBasic;
using StageCode.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static StageCode.LIB.OrthoAD;
using static StageCode.Other.Langs;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Xml.Linq;

namespace StageCode.LIB
{
    public partial class OrthoLabel : UserControl
    {
        private string _captionValues = "OrthoLabel";
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        private TDesign _TypeDesign = TDesign.Rectangle; // Type d'entrée du champ (cf. Classe TDesign)
        private string _Format; // Format de la variable (précision)
        private string _comment = ""; // Commentaire sur le contrôle
        private int _BorderW = 0;
        public System.Windows.Forms.Label Label ;
        private List<Color> _backColorList = new List<Color>();
        private List<Color> _foreColorList = new List<Color>();
        private string _visibility = "1";

        /// <summary>
        /// Param non utilisé mais visible au cas où
        /// </summary>
        private string[] _VL = new string[9];

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoLabel()
        {
            InitializeComponent();

            Label = Label1;
        }

        private void OrthoLabel_Load(object sender, EventArgs e)
        {
            Langs.LanguageChanged += LanguageChangedEventHandler;

            // Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            this.AutoSize = false;
            Label1.AutoSize = false;
            Label1.Text = this.Name;
            ControlUtils.RegisterControl(Label1, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoResult_Resize;
            base.Load += OrthoLabel_Load;
        }

        private void LanguageChangedEventHandler(AvailableLanguage NewLanguage)
        {
            if (_captionValues.Contains("|"))
            {
                string[] str = _captionValues.Split("|");
                var idx = NewLanguage.LanguageID;
                if (str.Length > idx)
                {
                    Label1.Text = str[idx];
                }
                else
                {
                    Label1.Text = str[0];
                }
            }
            else
            {
                Label1.Text = _captionValues;
            }
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

        public OrthoLabel ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);
            OrthoLabel orthoLabelControl = new OrthoLabel();

            // Parse le type et le nom de l'objet
            orthoLabelControl.Name = xml.Attribute("name")?.Value;

            // Parse la section <Apparence>
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                // Extraire les couleurs de fond et de texte multiples
                orthoLabelControl._backColorList = appearance.Element("MultiBackColor")?
                    .Elements("Color")
                    .Select(c => Color.FromArgb(int.Parse(c.Value)))
                    .ToList() ?? new List<Color>();

                orthoLabelControl._foreColorList = appearance.Element("MultiForeColor")?
                    .Elements("Color")
                    .Select(c => Color.FromArgb(int.Parse(c.Value)))
                    .ToList() ?? new List<Color>();

                // Extraire les couleurs principales
                orthoLabelControl.BackColor = Color.FromArgb(int.Parse(appearance.Element("BackColor")?.Value ?? "0"));
                orthoLabelControl.ForeColor = Color.FromArgb(int.Parse(appearance.Element("ForeColor")?.Value ?? "0"));

                // Extraire la taille et la localisation
                orthoLabelControl.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "100")
                );
                orthoLabelControl.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")
                );

                // Extraire les autres propriétés
                orthoLabelControl.Caption = appearance.Element("Caption")?.Value ?? string.Empty;
                orthoLabelControl.TextAlign = ContentAlignment_Parser.Get_Alignment(int.Parse(appearance.Element("TextAlign")?.Value ?? "MiddleCenter"));
                orthoLabelControl.Format = appearance.Element("Format")?.Value ?? string.Empty;

                // Extraire les propriétés de la police
                orthoLabelControl.Font = new Font(
                    appearance.Element("FontName")?.Value ?? "Arial",
                    float.Parse(appearance.Element("FontSize")?.Value ?? "12"),
                    FontStyle.Regular
                );

                if (bool.TryParse(appearance.Element("FontBold")?.Value, out bool fontBold) && fontBold)
                    orthoLabelControl.Font = new Font(orthoLabelControl.Font, orthoLabelControl.Font.Style | FontStyle.Bold);

                if (bool.TryParse(appearance.Element("FontItalic")?.Value, out bool fontItalic) && fontItalic)
                    orthoLabelControl.Font = new Font(orthoLabelControl.Font, orthoLabelControl.Font.Style | FontStyle.Italic);

                if (bool.TryParse(appearance.Element("FontStrikeout")?.Value, out bool fontStrikeout) && fontStrikeout)
                    orthoLabelControl.Font = new Font(orthoLabelControl.Font, orthoLabelControl.Font.Style | FontStyle.Strikeout);

                if (bool.TryParse(appearance.Element("FontUnderline")?.Value, out bool fontUnderline) && fontUnderline)
                    orthoLabelControl.Font = new Font(orthoLabelControl.Font, orthoLabelControl.Font.Style | FontStyle.Underline);

                // Extraire les autres propriétés spécifiques
                orthoLabelControl.TypeDesign = (TDesign)int.Parse(appearance.Element("TypeDesign")?.Value ?? "0");
                orthoLabelControl.BorderWidth = int.Parse(appearance.Element("BorderWidth")?.Value ?? "1");
                orthoLabelControl.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Value ?? "0");
                orthoLabelControl.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Value ?? "0");
                orthoLabelControl.Visibility = appearance.Element("Visibility")?.Value ?? "Visible";

                // Extraire les couleurs spécifiques
                orthoLabelControl.ColorOn = Color.FromArgb(int.Parse(appearance.Element("ColorOn")?.Value ?? "0"));
                orthoLabelControl.ColorOff = Color.FromArgb(int.Parse(appearance.Element("ColorOff")?.Value ?? "0"));
                orthoLabelControl.ColorErr = Color.FromArgb(int.Parse(appearance.Element("ColorErr")?.Value ?? "0"));

                // Extraire les valeurs VL
                var vlElements = appearance.Elements().Where(e => e.Name.ToString().StartsWith("VL"));
                orthoLabelControl._VL = new string[vlElements.Count()];
                foreach (var vl in vlElements)
                {
                    int index = int.Parse(vl.Name.ToString().Replace("VL", ""));
                    orthoLabelControl._VL[index] = vl.Value;
                }
            }

            return orthoLabelControl;
        }

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
            Caption = splitPvirgule[2];
            this.BackColor = FromOle(splitPvirgule[5]);
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
            _VL[0] = splitPvirgule[19];

            // Voir la propriété MultiBackColor
            _backColorList = new List<Color>();
            if (_VL[0] is not null)
            {
                string[] vals = _VL[0].Split('|');
                foreach (var V in vals)
                {
                    if (V is not null && !string.IsNullOrEmpty(V))
                    {
                        _backColorList.Add(FromOle(V));
                    }
                }
            }

            _VL[1] = splitPvirgule[20];

            // Voir la propriété MultiForeColor
            _foreColorList = new List<Color>();
            if (_VL[1] is not null)
            {
                string[] vals = _VL[1].Split('|');
                foreach (var V in vals)
                {
                    if (V is not null && !string.IsNullOrEmpty(V))
                    {
                        _foreColorList.Add(FromOle(V));
                    }
                }
            }

            _VL[2] = splitPvirgule[21];
            _VL[3] = splitPvirgule[22];
            _VL[4] = splitPvirgule[23];
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
            // Voir la propriété MultiBackColor
            string val = "";
            bool first = true;
            foreach (Color C in _backColorList)
            {
                if (!first)
                {
                    val += "|";
                }
                val += ToOle(C);
                first = false;
            }
            _VL[0] = val;

            // Voir la propriété MultiForeColor
            val = "";
            first = true;
            foreach (Color C in _foreColorList)
            {
                if (!first)
                {
                    val += "|";
                }
                val += ToOle(C);
                first = false;
            }
            _VL[1] = val;

            return "ORTHO;LABEL;" + Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(TextAlign).ToString() + ";" + Format + ";" + ToOle(this.BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + _VL[0] + ";" + _VL[1] + ";" + _VL[2] + ";" + _VL[3] + ";" + _VL[4] + ";" + _VL[5] + ";" + _VL[6] + ";" + _VL[7] + ";" + _VL[8] + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;







        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("      <Apparence>");

            // MultiBackColor
            xmlContent.AppendLine("        <MultiBackColor>");
            bool first = true;
            foreach (Color C in _backColorList)
            {
                if (!first)
                {
                    xmlContent.AppendLine("          <Color>" + ToOle(C) + "</Color>");
                }
                else
                {
                    xmlContent.AppendLine("          <Color>" + ToOle(C) + "</Color>");
                    first = false;
                }
            }
            xmlContent.AppendLine("        </MultiBackColor>");

            // MultiForeColor
            xmlContent.AppendLine("        <MultiForeColor>");
            first = true;
            foreach (Color C in _foreColorList)
            {
                if (!first)
                {
                    xmlContent.AppendLine("          <Color>" + ToOle(C) + "</Color>");
                }
                else
                {
                    xmlContent.AppendLine("          <Color>" + ToOle(C) + "</Color>");
                    first = false;
                }
            }
            xmlContent.AppendLine("        </MultiForeColor>");

            // Other properties
            xmlContent.AppendLine($"        <Caption>{Caption}</Caption>");
            xmlContent.AppendLine($"        <TextAlign>{ContentAlignment_Parser.Get_ValueToWrite(TextAlign)}</TextAlign>");
            xmlContent.AppendLine($"        <Format>{Format}</Format>");
            xmlContent.AppendLine($"        <BackColor>{ToOle(this.BackColor)}</BackColor>");
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

            // _VL values
            for (int i = 0; i < _VL.Length; i++)
            {
                xmlContent.AppendLine($"        <VL{i}>{_VL[i]}</VL{i}>");
            }

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

        #region Label Properties
        [Category("Apparence")]
        [Description("Texte du Label")]
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

        #region MultiCouleurs
        // Ces deux propriétés permettent de créer une liste de couleurs.
        // Elles sont à utiliser conjointement avec les propriétés Texts et Texts Index variable
        // En gros Texts contient les différentes chaînes de textes 
        // et Texts Index Variable contient le nom d'une variable
        // Lorsque la valeur de la variable changera, l'index d'affichage aussi et on pourra changer le texte et les couleurs du Label...
        // Trop bien... Vive le Chromdyne 4 qu'on arrête avec ces conneries...
        [Category("Multi")]
        [DisplayName("Multi BackColor")]
        [Description("BackColor in MultiText mode")]
        public List<Color> MultiBackColor
        {
            get
            {
                return _backColorList;
            }
            set
            {
                _backColorList = value;
            }
        }
        [Category("Multi")]
        [DisplayName("Multi Forecolor")]
        [Description("ForeColor in MultiText mode")]
        public List<Color> MultiForeColor
        {
            get
            {
                return _foreColorList;
            }
            set
            {
                _foreColorList = value;
            }
        }
        #endregion

        #region Orthodyne Properties
        // Voir la propriété MultiBackColor
        [Browsable(false)]
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

        // Voir la propriété MultiforeColor
        [Browsable(false)]
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
        [Category("Multi")]
        [DisplayName("Texts")]
        [ShowOnProtectedMode()]
        [Description("Texts that will be defined by a variable (and not by #IDX_LANG)")]
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
        [Category("Multi")]
        [DisplayName("Texts index Variable")]
        [Description("Variable that will define the index for the texts")]
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
                return "LABEL";
            }
        }

        public Type GType()
        {
            return GetType();
        }
    }
}
