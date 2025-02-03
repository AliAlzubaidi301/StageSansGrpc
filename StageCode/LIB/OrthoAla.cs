﻿using StageCode.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static StageCode.LIB.OrthoAD;

namespace StageCode.LIB
{
    public partial class OrthoAla :UserControl
    {
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        private TDesign _TypeDesign = TDesign.Bouton; // Type d'entrée du champ (cf. Classe TDesign)
        private string _Format; // Format de la variable (précision)
        private string _comment = ""; // Commentaire sur le contrôle
        private int _BorderW;
        private string _captionValues = "OrthoAla";
        private string _visibility = "1";

        private CButton btn = new CButton();
        private string _Commande; // Commande executée lors du clic sur le bouton
        private string _job = ""; // Nom du job a démarrer par la commande STARTJOB 
        private string _VarLink9;
        private string _VarLink2;
        private string _Etat = ""; // Nom de la Variable qui donne l'état de l'alarme
                                   // Private _VarClign As String = "" 'Nom de la Variable qui détermine le mode clignotant
        private string _NomFichier = ""; // Nom du fichier a charger lors du clic
        /// <summary>
        /// Param non utilisé mais visible au cas où
        /// </summary>
        private string[] _VL = new string[9];

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;
        public OrthoAla()
        {
            // Initialisation de l'interface graphique
            InitializeComponent();

            // Configuration du bouton
            btn.Text = Name;
            btn.FillType = CButton.eFillType.Solid;
            btn.BackColor = Color.Transparent;
            btn.Shape = CButton.eShape.Rectangle;
            btn.Corners.All = 0;

            // Ajout du bouton au formulaire
            this.Controls.Add(btn);

            // Paramètres du formulaire
            this.BackColor = Color.Transparent;

            // Abonnement aux événements
            Langs.LanguageChanged += LanguageChangedEventHandler;

            // Initialisation de la langue courante
            LanguageChangedEventHandler(Langs.CurrentLanguage);

            // Gestion de la visibilité avec ControlUtils
            ControlUtils.RegisterControl(
                btn,
                () => Visibility,
                h => VisibilityChanging += h,
                h => VisibilityChanged += h
            );

            this.Resize += OrthoAla_Resize;
            this.Load += OrthoAla_Load;
        }

        private void LanguageChangedEventHandler(AvailableLanguage NewLanguage)
        {
            if ((bool)(NewLanguage == null))
            {
                return; // Sécurité contre les valeurs nulles
            }

            if (!string.IsNullOrEmpty(_captionValues) && _captionValues.Contains("|"))
            {
                string[] str = _captionValues.Split('|'); // Utilisation correcte de Split()
                int idx = NewLanguage.LanguageID;

                btn.Text = (idx >= 0 && idx < str.Length) ? str[idx] : str[0];
            }
            else
            {
                btn.Text = _captionValues;
            }
        }

        private void OrthoAla_Resize(object sender, EventArgs e)
        {
            btn.Size = this.Size;
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
            return ColorTranslator.FromOle(int.Parse(DataIn)) ;
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
            if (splitPvirgule == null || splitPvirgule.Length < 34)
            {
                throw new ArgumentException("Le tableau splitPvirgule est null ou ne contient pas assez d'éléments.");
            }

            var StyleText = FontStyle.Regular;

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

            Name = $"{splitPvirgule[1]}_{splitPvirgule[2]}";
            Caption = splitPvirgule[2];
            Precision = splitPvirgule[4];

            if (float.TryParse(splitPvirgule[8], NumberStyles.Float, CultureInfo.InvariantCulture, out float fontSize))
            {
                Font = new Font(splitPvirgule[7], fontSize, StyleText);
            }

            BackColor = FromOle(splitPvirgule[5]);
            ForeColor = FromOle(splitPvirgule[6]);

            if (Enum.TryParse(splitPvirgule[13], out TDesign typeDesign))
            {
                TypeDesign = typeDesign;
            }

            if (int.TryParse(splitPvirgule[14], out int borderWidth))
            {
                BorderWidth = borderWidth;
            }

            if (int.TryParse(splitPvirgule[16], out int width) && int.TryParse(splitPvirgule[15], out int height))
            {
                Size = new Size(width, height);
            }

            if (int.TryParse(splitPvirgule[18], out int x) && int.TryParse(splitPvirgule[17], out int y))
            {
                Location = FromCopy ? new Point(x + 10, y + 10) : new Point(x, y);
            }

            this.comment = comment;
            Etat = splitPvirgule[19];
            VarLink2 = splitPvirgule[20];
            NomFichier = splitPvirgule[21];

            _VL[3] = splitPvirgule[22];
            _VL[4] = splitPvirgule[23];
            _VL[5] = splitPvirgule[24];
            _VL[6] = splitPvirgule[25];

            Commande = splitPvirgule[26];
            VarLink9 = splitPvirgule[27];

            ColorOn = FromOle(splitPvirgule[28]);
            ColorOff = FromOle(splitPvirgule[29]);
            ColorErr = FromOle(splitPvirgule[30]);

            if (int.TryParse(splitPvirgule[31], out int levelVisible))
            {
                LevelVisible = levelVisible;
            }

            if (int.TryParse(splitPvirgule[32], out int levelEnabled))
            {
                LevelEnabled = levelEnabled;
            }

            if (splitPvirgule.Length >= 34)
            {
                Visibility = splitPvirgule[33];
            }

            return this;
        }

        public string WriteFile()
        {
            return "ORTHO;ALA;" + Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(TextAlign).ToString() + ";" + Precision + ";" + ToOle(BackColor).ToString() + ";" + ToOle(ForeColor).ToString() + ";" + Font.Name.ToString() + ";" + Font.Size.ToString() + ";" + Font.Strikeout.ToString() + ";" + Font.Underline.ToString() + ";" + Font.Bold.ToString() + ";" + Font.Italic.ToString() + ";" + Convert.ToInt32(TypeDesign).ToString() + ";" + BorderWidth.ToString() + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + Etat + ";" + VarLink2 + ";" + NomFichier + ";" + _VL[3] + ";" + _VL[4] + ";" + _VL[5] + ";" + _VL[6] + ";" + Commande + ";" + VarLink9 + ";" + ToOle(ColorOn).ToString() + ";" + ToOle(ColorOff).ToString() + ";" + ToOle(ColorErr).ToString() + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + Visibility;
        }
        #endregion



        #region Control Properties
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
        [Category("Orthodyne")]
        [Description("Variable qui donne l'état de l'alarme")]
        public string Etat
        {
            get
            {
                return _Etat;
            }
            set
            {
                _Etat = value;
            }
        }
        [Category("Orthodyne")]
        [Description("Nom du fichier a charger si clic sur l'alarme (optionnel)")]
        public string NomFichier
        {
            get
            {
                return _NomFichier;
            }
            set
            {
                _NomFichier = value;
            }
        }
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
        [Category("Apparence")]
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
        // Supprimé car complètement redondant avec Property Caption...
        // <Category("Apparence"), Description("Caption du bouton"), Editor(GetType(OrthoMultiLanguageEditor), GetType(UITypeEditor))> _
        // Overrides Property Text() As String
        // Get
        // Return _captionValues
        // End Get
        // Set(ByVal value As String)
        // _captionValues = value
        // LanguageChangedEventHandler(Langs.CurrentLanguage)
        // End Set
        // End Property
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
        [Description("Option n°2 dépendant des autres valeurs")]
        public string VarLink2
        {
            get
            {
                return _VarLink2;
            }
            set
            {
                _VarLink2 = value;
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
        [Category("Orthodyne")]
        [Description("utilisation variant selon d'autre params")]
        public string VarLink9
        {
            get
            {
                return _VarLink9;
            }
            set
            {
                _VarLink9 = value;
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
                return "ALA";
            }
        }

        public Type GType()
        {
            return GetType();
        }
        public string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private void OrthoAla_Load(object sender, EventArgs e)
        {

        }
    }
}
