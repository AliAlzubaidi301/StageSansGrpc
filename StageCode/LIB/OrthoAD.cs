using StageCode.Other;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;

namespace StageCode.LIB
{
    public partial class OrthoAD : UserControl
    {
        private void OrthoAD_Load(object sender, EventArgs e)
        {

        }
        public enum TDesign
        {
            Bouton = 1,
            Rectangle,
            Cercle,
            Arrondi,
            Tedit,
            Combo
        }

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

        // Param non utilisé mais visible au cas où
        private string[] _VL = new string[8];

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoAD()
        {
            InitializeComponent();

            // Ajout du bouton au contrôle
            this.Controls.Add(btn);
            btn.Text = this.Name;
            this.BackColor = Color.Transparent;
            btn.FillType = CButton.eFillType.Solid; // Remplacez par la classe CButtonLib.CButton si vous l'utilisez
            btn.Shape = CButton.eShape.Rectangle; // Même remarque ici pour CButtonLib.CButton
            btn.Corners.All = 6;
            // Inscription des gestionnaires d'événements
            ControlUtils.RegisterControl(btn, () => Visibility, h => VisibilityChanging += h,h => VisibilityChanged += h);


        }

        private void OrthoResult_Resize(object sender, EventArgs e)
        {
            if (btn.Shape == CButton.eShape.Ellipse) // Vérifie si la forme est une ellipse
            {
                btn.Size = new Size(this.Height, this.Height); // Définir la taille du bouton en fonction de la hauteur du formulaire
                this.Size = new Size(this.Height, this.Height); // Définir la taille du formulaire selon la hauteur
            }
            else
            {
                btn.Size = this.Size; // Sinon, définir la taille du bouton à la taille du formulaire
            }
        }
        /// <summary>
        /// Conversion des couleurs dans différents modes d'encodage
        /// </summary>
        /// <param name="DataIn">Couleur en format Ole</param>
        /// <returns>Couleur au format .Net</returns>
        private Color FromOle(string DataIn)
        {
            // Gestion de la transparence à la Orthodyne
            if (DataIn == "-1")
            {
                return Color.Transparent;
            }
            return ColorTranslator.FromOle(int.Parse(DataIn));
        }
        /// <summary>
        /// Conversion des couleurs dans différents modes d'encodage
        /// </summary>
        /// <param name="DataIn">Couleur en format .Net</param>
        /// <returns>Couleur au format Ole</returns>
        private string ToOle(Color DataIn)
        {
            if (DataIn == Color.Transparent)
            {
                return "-1";
            }
            return ColorTranslator.ToOle(DataIn).ToString();
        }
        #region("Read/Write on .syn file")

        public OrthoAD ReadFile(string[] splitPvirgule, string comment, string file, bool FromCopy)
        {
            FontStyle styleText = FontStyle.Regular;
            this.TextAlign = ContentAlignment_Parser.Get_Alignment(int.Parse(splitPvirgule[3]));

            if (splitPvirgule[9] == "True")
            {
                styleText |= FontStyle.Strikeout;
            }
            if (splitPvirgule[10] == "True")
            {
                styleText |= FontStyle.Underline;
            }
            if (splitPvirgule[11] == "True")
            {
                styleText |= FontStyle.Bold;
            }
            if (splitPvirgule[12] == "True")
            {
                styleText |= FontStyle.Italic;
            }

            this.Name = splitPvirgule[1] + "_" + splitPvirgule[2];
            this.Comment = comment;
            this.Caption = splitPvirgule[2];
            this.Format = splitPvirgule[4];
            this.Font = new Font(splitPvirgule[7], float.Parse(splitPvirgule[8]), styleText);
            this.Text = splitPvirgule[2];
            this.BackColor = FromOle(splitPvirgule[5]);
            this.ForeColor = FromOle(splitPvirgule[6]);
            this.TypeDesign = (TDesign)Enum.Parse(typeof(TDesign), splitPvirgule[13]); // ⚠️ Peut lever une exception si la valeur est invalide
            this.BorderWidth = int.Parse(splitPvirgule[14]);
            this.Size = new Size(int.Parse(splitPvirgule[16]), int.Parse(splitPvirgule[15]));

            if (FromCopy)
            {
                this.Location = new Point(int.Parse(splitPvirgule[18]) + 10, int.Parse(splitPvirgule[17]) + 10);
            }
            else
            {
                this.Location = new Point(int.Parse(splitPvirgule[18]), int.Parse(splitPvirgule[17]));
            }

            this.VarText = splitPvirgule[19];
            this.VarBackColor = splitPvirgule[20];
            this.VarForeColor = splitPvirgule[21];
            this.VarValMax = splitPvirgule[22];
            this.VarTextMax = splitPvirgule[23];
            this.VarValMin = splitPvirgule[24];
            this.VarTextMin = splitPvirgule[25];
            this._VL[7] = splitPvirgule[26];
            this._VL[8] = splitPvirgule[27];
            this.ColorOn = FromOle(splitPvirgule[28]);
            this.ColorOff = FromOle(splitPvirgule[29]);
            this.ColorErr = FromOle(splitPvirgule[30]);
            this.LevelVisible = int.Parse(splitPvirgule[31]);
            this.LevelEnabled = int.Parse(splitPvirgule[32]);

            if (splitPvirgule.Length >= 34)
            {
                this.Visibility = splitPvirgule[33];
            }

            return this;
        }
        public string WriteFile()
        {
            return "ORTHO;AD;" + this.Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(this.TextAlign).ToString() + ";" + this.Format +
                ";" + ToOle(this.BackColor).ToString() + ";" + ToOle(this.ForeColor).ToString() + ";" + this.Font.Name.ToString() +
                ";" + this.Font.Size.ToString() + ";" + this.Font.Strikeout.ToString() +
                ";" + this.Font.Underline.ToString() + ";" + this.Font.Bold.ToString() + ";" + this.Font.Italic.ToString() +
                ";" + Convert.ToInt32(this.TypeDesign).ToString() + ";" + this.BorderWidth.ToString() + ";" + this.Size.Height.ToString() +
                ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() +
                ";" + this.VarText + ";" + this.VarBackColor + ";" + this.VarForeColor + ";" + this.VarValMax + ";" + this.VarTextMax + ";" + this.VarValMin + ";" + this.VarTextMin + ";" + this._VL[7] + ";" + this._VL[8] + ";" + ToOle(this.ColorOn).ToString() +
                ";" + ToOle(this.ColorOff).ToString() + ";" + ToOle(this.ColorErr).ToString() + ";" + this.LevelVisible.ToString() +
                ";" + this.LevelEnabled.ToString() + ";" + this.Visibility;
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

        #endregion

        #region "Label Properties"
        [Category("Apparence"), Description("Détermine la position du texte dans ce contrôle")]
        public ContentAlignment TextAlign
        {
            get { return btn.TextAlign; }
            set { btn.TextAlign = value; }
        }

        [Category("Apparence"), ShowOnProtectedMode(), Description("La couleur d'arrière plan du contrôle.")]
        public override Font Font
        {
            get { return btn.Font; }
            set { btn.Font = value; }
        }

        [Category("Apparence"), Description("La couleur d'arrière plan du contrôle.")]
        public override Color ForeColor
        {
            get { return btn.ForeColor; }
            set { btn.ForeColor = value; }
        }

        public string Caption
        {
            get { return _captionValues; }
            set
            {
                _captionValues = value;
                LanguageChangedEventHandler(Langs.CurrentLanguage);
            }
        }

        private void LanguageChangedEventHandler(AvailableLanguage NewLanguage)
        {
            if (_captionValues.Contains("|"))
            {
                string[] str = _captionValues.Split('|');
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


        public override Color BackColor
        {
            get { return btn.BackColor; }
            set { btn.BackColor = value; }
        }
        #endregion

        #region "Orthodyne Properties"
        [Category("Not assigned"), Description("Variable interne non utilisé actuellement")]
        public string VarLink8
        {
            get { return _VL[7]; }
            set { _VL[7] = value; }
        }

        [Category("Not assigned"), Description("Variable interne non utilisé actuellement")]
        public string VarLink9
        {
            get { return _VL[8]; }
            set { _VL[8] = value; }
        }

        [Category("Affichage conditionnel"), ShowOnProtectedMode(), Description("Var ou valeur qui définit la valeur maximum.")]
        public string VarValMax
        {
            get { return _VarValMax; }
            set { _VarValMax = value; }
        }

        public string VarTextMax
        {
            get { return _VarTextMax; }
            set { _VarTextMax = value; }
        }

        public string VarTextMin
        {
            get { return _VarTextMin; }
            set { _VarTextMin = value; }
        }

        [Category("Affichage conditionnel"), ShowOnProtectedMode(), Description("Var ou valeur qui définit la valeur minimum.")]
        public string VarValMin
        {
            get { return _VarValMin; }
            set { _VarValMin = value; }
        }

        [Category("Apparence"), Description("Taille de la bordure du contrôle")]
        public int BorderWidth
        {
            get { return _BorderW; }
            set { _BorderW = value; }
        }

        [Category("Orthodyne"), Browsable(false), Description("Commentaire sur l'objet")]
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public string Format
        {
            get { return _Format; }
            set { _Format = value; }
        }

        [Category("Orthodyne"), Description("Indique quel type de design utiliser")]
        public TDesign TypeDesign
        {
            get { return _TypeDesign; }
            set
            {
                _TypeDesign = value;
                switch (value)
                {
                    case TDesign.Bouton:
                        btn.Shape = CButton.eShape.Rectangle;
                        btn.Corners.All = 0;
                        break;
                    case TDesign.Arrondi:
                        btn.Shape = CButton.eShape.Rectangle;
                        btn.Corners.All = 6;
                        break;
                    case TDesign.Rectangle:
                        btn.Shape = CButton.eShape.Rectangle;
                        btn.Corners.All = 0;
                        break;
                    case TDesign.Cercle:
                        btn.Shape = CButton.eShape.Ellipse;
                        btn.Size = new Size(this.Height, this.Height);
                        this.Size = btn.Size;
                        break;
                }
            }
        }

        [Category("Orthodyne"), Description("Niveau minimum pour rendre l'objet visible (si 0, toujours visible)")]
        public int LevelVisible
        {
            get { return _LevelVisible; }
            set { _LevelVisible = value; }
        }

        [Category("Orthodyne"), Description("Niveau minimum pour rendre l'objet accessible (si 0, toujours accessible)")]
        public int LevelEnabled
        {
            get { return _LevelEnabled; }
            set { _LevelEnabled = value; }
        }

        [Category("Orthodyne"), Description("Couleur lorsque actif")]
        public Color ColorOn
        {
            get { return _ColorOn; }
            set { _ColorOn = value; }
        }

        [Category("Orthodyne"), Description("Couleur lorsque non actif")]
        public Color ColorOff
        {
            get { return _ColorOff; }
            set { _ColorOff = value; }
        }

        [Category("Orthodyne"), Description("Couleur lorsque en erreur")]
        public Color ColorErr
        {
            get { return _ColorErr; }
            set { _ColorErr = value; }
        }

        [Category("Texte"), Description("Nom de la variable qui donne le texte.")]
        public string VarText
        {
            get { return _VarText; }
            set { _VarText = value; }
        }

        [Category("Texte"), Description("Nom de la variable qui donne la couleur d'arrière plan.")]
        public string VarBackColor
        {
            get { return _VarBackColor; }
            set { _VarBackColor = value; }
        }

        [Category("Texte"), Description("Nom de la variable qui donne la couleur du texte.")]
        public string VarForeColor
        {
            get { return _VarForeColor; }
            set { _VarForeColor = value; }
        }

        [Category("Orthodyne")]
        [Description("If 0 or will be hidden, if #VarName will depend on variable value")]
        public string Visibility
        {
            get { return _visibility; }
            set
            {
                VisibilityChanging?.Invoke(this, EventArgs.Empty);

                // Set the new value of _visibility
                _visibility = string.IsNullOrEmpty(value) ? "1" : value;

                // Raise the VisibilityChanged event
                VisibilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region "Hiding useless Properties"
        [Browsable(false)]
        public AccessibleRole AccessibleRole
        {
            get { return base.AccessibleRole; }
            set { base.AccessibleRole = value; }
        }

        [Browsable(false)]
        public string AccessibleDescription
        {
            get { return AccessibleDescription; }
            set { base.AccessibleDescription = value; }
        }

        [Browsable(false)]
        public string AccessibleName
        {
            get { return AccessibleName; }
            set { AccessibleName = value; }
        }

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get { return BackgroundImage; }
            set { BackgroundImage = value; }
        }

        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return BackgroundImageLayout; }
            set { BackgroundImageLayout = value; }
        }

        [Browsable(false)]
        public BorderStyle BorderStyle
        {
            get { return BorderStyle; }
            set { BorderStyle = value; }
        }


        [Browsable(false)]
        public override Cursor Cursor
        {
            get { return Cursor; }
            set { Cursor = value; }
        }

        [Browsable(false)]
        public override RightToLeft RightToLeft
        {
            get { return RightToLeft; }
            set { RightToLeft = value; }
        }

        [Browsable(false)]
        public override bool AllowDrop
        {
            get { return AllowDrop; }
            set { base.AllowDrop = value; }
        }

        [Browsable(false)]
        public override AutoValidate AutoValidate
        {
            get { return base.AutoValidate; }
            set { base.AutoValidate = value; }
        }

        [Browsable(false)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return ContextMenuStrip; }
            set { base.ContextMenuStrip = value; }
        }

        [Browsable(false)]
        public bool Enabled
        {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }


        [Browsable(false)]
        public ImeMode ImeMode
        {
            get { return base.ImeMode; }
            set { base.ImeMode = value; }
        }

        [Browsable(false)]
        public int TabIndex
        {
            get { return base.TabIndex; }
            set { base.TabIndex = value; }
        }

        [Browsable(false)]
        public bool TabStop
        {
            get { return base.TabStop; }
            set { base.TabStop = value; }
        }

        [Browsable(false)]
        public bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        [Browsable(false)]
        public override AnchorStyles Anchor
        {
            get { return base.Anchor; }
            set { base.Anchor = value; }
        }

        [Browsable(false)]
        public override bool AutoScroll
        {
            get { return base.AutoScroll; }
            set { base.AutoScroll = value; }
        }

        [Browsable(false)]
        public Size AutoScrollMargin
        {
            get { return base.AutoScrollMargin; }
            set { base.AutoScrollMargin = value; }
        }

        [Browsable(false)]
        public Size AutoScrollMinSize
        {
            get { return base.AutoScrollMinSize; }
            set { base.AutoScrollMinSize = value; }
        }

        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ShowOnProtectedModeAttribute : Attribute
    {
    }
}
