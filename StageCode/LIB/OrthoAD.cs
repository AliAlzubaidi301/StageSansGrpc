using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace StageCode.LIB
{
    public partial class OrthoAD : UserControl
    {
        private void OrthoAD_Load(object sender, EventArgs e)
        {

        }
        //    public enum TDesign
        //    {
        //        Bouton = 1,
        //        Rectangle,
        //        Cercle,
        //        Arrondi,
        //        Tedit,
        //        Combo
        //    }

        //    private string _captionValues = "OrthoAD";
        //    private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        //    private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        //    private Color _ColorOn = Color.Lime; // Couleur du contrôle lorsqu'il est actif
        //    private Color _ColorOff = Color.Red; // Couleur du contrôle lorsqu'il est inactif
        //    private Color _ColorErr = Color.FromArgb(207, 192, 192); // Couleur du contrôle lorsqu'il est en erreur
        //    private TDesign _TypeDesign = TDesign.Arrondi; // Type d'entrée du champ (cf. Classe TDesign)
        //    private string _Format; // Format de la variable (précision)
        //    private string _comment = ""; // Commentaire sur le contrôle
        //    private int _BorderW = 0;

        //    private Button btn = new Button();
        //    private string _VarText; // Varlink 1
        //    private string _VarForeColor; // Varlink 3
        //    private string _VarBackColor; // Varlink 2
        //    private string _VarValMax; // Varlink 4 : Reference Max
        //    private string _VarTextMax; // Varlink 5 : Texte affiche si plus haut
        //    private string _VarValMin; // Varlink 6 : Reference Min
        //    private string _VarTextMin; // Varlink 7 : Texte affiche si plus petit que valmin
        //    private string _visibility = "1";

        //    // Param non utilisé mais visible au cas où
        //    private string[] _VL = new string[8];

        //    public event EventHandler VisibilityChanging;
        //    public event EventHandler VisibilityChanged;

        //    public OrthoAD()
        //    {
        //        InitializeComponent();

        //        // Ajout du bouton au contrôle
        //        this.Controls.Add(btn);
        //        btn.Text = this.Name;
        //        this.BackColor = Color.Transparent;
        //        btn.FillType = CButtonLib.CButton.eFillType.Solid; // Remplacez par la classe CButtonLib.CButton si vous l'utilisez
        //        btn.Shape = CButtonLib.CButton.eShape.Rectangle; // Même remarque ici pour CButtonLib.CButton
        //        btn.Corners.All = 6;

        //        // Inscription des gestionnaires d'événements
        //        RegisterControl(btn, () => Visibility,
        //            h => VisibilityChanging += h,
        //            h => VisibilityChanged += h);
        //    }

        //    private void OrthoResult_Resize(object sender, EventArgs e)
        //    {
        //        if (btn.Shape == CButtonLib.CButton.eShape.Ellipse) // Vérifie si la forme est une ellipse
        //        {
        //            btn.Size = new Size(this.Height, this.Height); // Définir la taille du bouton en fonction de la hauteur du formulaire
        //            this.Size = new Size(this.Height, this.Height); // Définir la taille du formulaire selon la hauteur
        //        }
        //        else
        //        {
        //            btn.Size = this.Size; // Sinon, définir la taille du bouton à la taille du formulaire
        //        }
        //    }
        //    /// <summary>
        //    /// Conversion des couleurs dans différents modes d'encodage
        //    /// </summary>
        //    /// <param name="DataIn">Couleur en format Ole</param>
        //    /// <returns>Couleur au format .Net</returns>
        //    private Color FromOle(string DataIn)
        //    {
        //        // Gestion de la transparence à la Orthodyne
        //        if (DataIn == "-1")
        //        {
        //            return Color.Transparent;
        //        }
        //        return ColorTranslator.FromOle(int.Parse(DataIn));
        //    }
        //    /// <summary>
        //    /// Conversion des couleurs dans différents modes d'encodage
        //    /// </summary>
        //    /// <param name="DataIn">Couleur en format .Net</param>
        //    /// <returns>Couleur au format Ole</returns>
        //    private string ToOle(Color DataIn)
        //    {
        //        if (DataIn == Color.Transparent)
        //        {
        //            return "-1";
        //        }
        //        return ColorTranslator.ToOle(DataIn).ToString();
        //    }
        //    #region("Read/Write on .syn file")

        //    public OrthoAD ReadFile(string[] splitPvirgule, string comment, string file, bool FromCopy)
        //    {
        //        FontStyle StyleText = new FontStyle();
        //        this.TextAlign = ContentAlignment_Parser.Get_Alignment(int.Parse(splitPvirgule[3]));

        //        if (splitPvirgule[9] == "True")
        //        {
        //            StyleText |= FontStyle.Strikeout;
        //        }
        //        if (splitPvirgule[10] == "True")
        //        {
        //            StyleText |= FontStyle.Underline;
        //        }
        //        if (splitPvirgule[11] == "True")
        //        {
        //            StyleText |= FontStyle.Bold;
        //        }
        //        if (splitPvirgule[12] == "True")
        //        {
        //            StyleText |= FontStyle.Italic;
        //        }

        //        this.Name = splitPvirgule[1] + "_" + splitPvirgule[2];
        //        this.comment = comment;
        //        this.Caption = splitPvirgule[2];
        //        this.Format = splitPvirgule[4];
        //        this.Font = new Font(splitPvirgule[7], float.Parse(splitPvirgule[8]), StyleText);
        //        this.Text = splitPvirgule[2];
        //        this.BackColor = FromOle(splitPvirgule[5]);
        //        this.ForeColor = FromOle(splitPvirgule[6]);
        //        this.TypeDesign = splitPvirgule[13];
        //        this.BorderWidth = int.Parse(splitPvirgule[14]);
        //        this.Size = new Size(int.Parse(splitPvirgule[16]), int.Parse(splitPvirgule[15]));

        //        if (FromCopy)
        //        {
        //            this.Location = new Point(int.Parse(splitPvirgule[18]) + 10, int.Parse(splitPvirgule[17]) + 10);
        //        }
        //        else
        //        {
        //            this.Location = new Point(int.Parse(splitPvirgule[18]), int.Parse(splitPvirgule[17]));
        //        }

        //        this.VarText = splitPvirgule[19];
        //        this.VarBackColor = splitPvirgule[20];
        //        this.VarForeColor = splitPvirgule[21];
        //        this.VarValMax = splitPvirgule[22];
        //        this.VarTextMax = splitPvirgule[23];
        //        this.VarValMin = splitPvirgule[24];
        //        this.VarTextMin = splitPvirgule[25];
        //        this._VL[7] = splitPvirgule[26];
        //        this._VL[8] = splitPvirgule[27];
        //        this.ColorOn = FromOle(splitPvirgule[28]);
        //        this.ColorOff = FromOle(splitPvirgule[29]);
        //        this.ColorErr = FromOle(splitPvirgule[30]);
        //        this.LevelVisible = int.Parse(splitPvirgule[31]);
        //        this.LevelEnabled = int.Parse(splitPvirgule[32]);

        //        if (splitPvirgule.Length >= 34)
        //        {
        //            this.Visibility = splitPvirgule[33];
        //        }

        //        return this;
        //    }
        //    public string WriteFile()
        //    {
        //        return "ORTHO;AD;" + this.Caption + ";" + ContentAlignment_Parser.Get_ValueToWrite(this.TextAlign).ToString() + ";" + this.Format +
        //            ";" + ToOle(this.BackColor).ToString() + ";" + ToOle(this.ForeColor).ToString() + ";" + this.Font.Name.ToString() +
        //            ";" + this.Font.Size.ToString() + ";" + this.Font.Strikeout.ToString() +
        //            ";" + this.Font.Underline.ToString() + ";" + this.Font.Bold.ToString() + ";" + this.Font.Italic.ToString() +
        //            ";" + Convert.ToInt32(this.TypeDesign).ToString() + ";" + this.BorderWidth.ToString() + ";" + this.Size.Height.ToString() +
        //            ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() +
        //            ";" + this.VarText + ";" + this.VarBackColor + ";" + this.VarForeColor + ";" + this.VarValMax + ";" + this.VarTextMax + ";" + this.VarValMin + ";" + this.VarTextMin + ";" + this._VL[7] + ";" + this._VL[8] + ";" + ToOle(this.ColorOn).ToString() +
        //            ";" + ToOle(this.ColorOff).ToString() + ";" + ToOle(this.ColorErr).ToString() + ";" + this.LevelVisible.ToString() +
        //            ";" + this.LevelEnabled.ToString() + ";" + this.Visibility;
        //    }
        //    #endregion

        //    #region("Label Properties")

        //    [Category("Appearance"), Description("Détermine la position du texte dans ce contrôle")]
        //    public ContentAlignment TextAlign
        //    {
        //        get
        //        {
        //            return btn.TextAlign;
        //        }
        //        set
        //        {
        //            btn.TextAlign = value;
        //        }
        //    }

        //    [Category("Appearance"), ShowOnProtectedMode(), Description("La couleur d'arrière plan du contrôle.")]
        //    public new Font Font
        //    {
        //        get
        //        {
        //            return btn.Font;
        //        }
        //        set
        //        {
        //            btn.Font = value;
        //        }
        //    }

        //    [Category("Appearance"), Description("La couleur d'arrière plan du contrôle.")]
        //    public new Color ForeColor
        //    {
        //        get
        //        {
        //            return btn.ForeColor;
        //        }
        //        set
        //        {
        //            btn.ForeColor = value;
        //        }
        //    }

        //    [Category("Appearance"), Description("Texte du label"), ShowOnProtectedMode(), Editor(typeof(OrthoMultiLanguageEditor), typeof(UITypeEditor))]
        //    public string Caption
        //    {
        //        get
        //        {
        //            return _captionValues;
        //        }
        //        set
        //        {
        //            _captionValues = value;
        //            LanguageChangedEventHandler(Langs.CurrentLanguage);
        //        }
        //    }

        //    public new Color BackColor
        //    {
        //        get
        //        {
        //            return btn.BackColor;
        //        }
        //        set
        //        {
        //            btn.BackColor = value;
        //        }
        //    }

        //#endregion

    }
}
