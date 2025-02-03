using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

public class INTEG : UserControl
{
    #region Orthodyn Data
    private int _levelVisible = 0;
    private int _levelEnabled = 0;
    private string _comment = "";
    private string _visibility = "1";
    #endregion

    #region Control Data
    private string _detecteur;
    #endregion

    public event EventHandler VisibilityChanging;
    public event EventHandler VisibilityChanged;

    public INTEG()
    {
        InitializeComponent();
        //RegisterControl(this, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
    }

    #region Read/Write on .syn file
    public INTEG ReadFile(string[] splitPvirgule, string comment, string file, bool fromCopy)
    {
        this.Comment = comment;
        this.Name = splitPvirgule[1];
        this.Size = new Size(int.Parse(splitPvirgule[3]), int.Parse(splitPvirgule[2]));
        this.Location = fromCopy
            ? new Point(int.Parse(splitPvirgule[5]) + 10, int.Parse(splitPvirgule[4]) + 10)
            : new Point(int.Parse(splitPvirgule[5]), int.Parse(splitPvirgule[4]));
        this.LevelVisible = int.Parse(splitPvirgule[7]);
        this.LevelEnabled = int.Parse(splitPvirgule[8]);
        this.Detecteur = splitPvirgule[6];
        if (splitPvirgule.Length >= 10)
        {
            this.Visibility = splitPvirgule[9];
        }
        return this;
    }

    public string WriteFile()
    {
        return $"INTEG;{Name};{Size.Height};{Size.Width};{Location.Y};{Location.X};{Detecteur};{LevelVisible};{LevelEnabled};{Visibility}";
    }
    #endregion

    #region Control Properties
    [Category("Orthodyne"), Description("Nom du détecteur associé au graph")]
    public string Detecteur
    {
        get => _detecteur;
        set => _detecteur = value;
    }
    #endregion

    #region Orthodyne Properties
    [Category("Orthodyne"), Browsable(false), Description("Commentaire sur l'objet")]
    public string Comment
    {
        get => _comment;
        set => _comment = value;
    }

    [Category("Orthodyne"), Description("Niveau minimum pour rendre l'objet visible (si 0, toujours visible)")]
    public int LevelVisible
    {
        get => _levelVisible;
        set => _levelVisible = value;
    }

    [Category("Orthodyne"), Description("Niveau minimum pour rendre l'objet accessible (si 0, toujours accessible)")]
    public int LevelEnabled
    {
        get => _levelEnabled;
        set => _levelEnabled = value;
    }

    [Category("Orthodyne"), Description("If 0 or will be hidden, if #VarName will depend on variable value")]
    public string Visibility
    {
        get => _visibility;
        set
        {
            VisibilityChanging?.Invoke(this, EventArgs.Empty);
            _visibility = string.IsNullOrEmpty(value) ? "1" : value;
            VisibilityChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    #endregion

    #region Hiding useless Properties
    [Browsable(false)] public override Color BackColor { get; set; }
    [Browsable(false)] public override Color ForeColor { get; set; }
    [Browsable(false)] public override Font Font { get; set; }
    [Browsable(false)] public override AccessibleRole AccessibleRole { get; set; }
    [Browsable(false)] public override string AccessibleDescription { get; set; }
    [Browsable(false)] public override string AccessibleName { get; set; }
    [Browsable(false)] public override Image BackgroundImage { get; set; }
    [Browsable(false)] public override ImageLayout BackgroundImageLayout { get; set; }
    [Browsable(false)] public override BorderStyle BorderStyle { get; set; }
    [Browsable(false)] public override Cursor Cursor { get; set; }
    [Browsable(false)] public override RightToLeft RightToLeft { get; set; }
    [Browsable(false)] public override bool UseWaitCursor { get; set; }
    [Browsable(false)] public override bool AllowDrop { get; set; }
    [Browsable(false)] public override AutoValidate AutoValidate { get; set; }
    [Browsable(false)] public override ContextMenuStrip ContextMenuStrip { get; set; }
    [Browsable(false)] public override bool Enabled { get; set; }
    [Browsable(false)] public override ImeMode ImeMode { get; set; }
    [Browsable(false)] public override int TabIndex { get; set; }
    [Browsable(false)] public override bool TabStop { get; set; }
    [Browsable(false)] public override bool Visible { get; set; }
    [Browsable(false)] public override AnchorStyles Anchor { get; set; }
    [Browsable(false)] public override bool AutoScroll { get; set; }
    [Browsable(false)] public override Size AutoScrollMargin { get; set; }
    [Browsable(false)] public override Size AutoScrollMinSize { get; set; }
    [Browsable(false)] public override bool AutoSize { get; set; }
    [Browsable(false)] public override AutoSizeMode AutoSizeMode { get; set; }
    [Browsable(false)] public override DockStyle Dock { get; set; }
    [Browsable(false)] public override Padding Margin { get; set; }
    [Browsable(false)] public override Size MaximumSize { get; set; }
    [Browsable(false)] public override Size MinimumSize { get; set; }
    [Browsable(false)] public override Padding Padding { get; set; }
    [Browsable(false)] public new ControlBindingsCollection DataBindings => base.DataBindings;
    [Browsable(false)] public override object Tag { get; set; }
    [Browsable(false)] public override bool CausesValidation { get; set; }
    #endregion

    public string Type => "INTEG";
    public string SType => "INTEG";

    public Type GType()
    {
        return this.GetType();
    }

    private void InitializeComponent() { }

    private void INTEG_Load(object sender, EventArgs e) { }
}
