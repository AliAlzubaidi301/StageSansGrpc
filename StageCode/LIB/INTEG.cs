using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace StageCode
{
    public sealed partial class INTEG : UserControl
    {
        #region Données Orthodyn
        private int _levelVisible;
        private int _levelEnabled;
        private string _comment = string.Empty;
        private string _visibility = "1";
        #endregion

        #region Données du contrôle
        private string _detecteur = string.Empty;
        #endregion

        // Événements pour la gestion de la visibilité
        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        // Constructeur
        public INTEG()
        {
            InitializeComponent();
        }

        #region Propriétés du contrôle
        [Category("Orthodyne"), Description("Nom du détecteur associé au graph")]
        public string Detecteur
        {
            get => _detecteur;
            set => _detecteur = value ?? string.Empty;
        }
        #endregion

        #region Propriétés Orthodyne
        [Category("Orthodyne"), Browsable(false), Description("Commentaire sur l'objet")]
        public string Comment
        {
            get => _comment;
            set => _comment = value ?? string.Empty;
        }

        [Category("Orthodyne"), Description("Niveau minimum pour rendre l'objet visible (si 0, toujours visible)")]
        public int LevelVisible
        {
            get => _levelVisible;
            set => _levelVisible = Math.Max(0, value);
        }

        [Category("Orthodyne"), Description("Niveau minimum pour rendre l'objet accessible (si 0, toujours accessible)")]
        public int LevelEnabled
        {
            get => _levelEnabled;
            set => _levelEnabled = Math.Max(0, value);
        }

        [Category("Orthodyne"), Description("Si 0, l'objet sera caché. Si #VarName, la visibilité dépendra de la valeur de la variable.")]
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

        #region Lecture/Écriture du fichier .syn
        public INTEG ReadFile(string[] splitPvirgule, string comment, string file, bool fromCopy)
        {
            if (splitPvirgule == null || splitPvirgule.Length < 9)
            {
                throw new ArgumentException("Le fichier est mal formé ou incomplet.", nameof(splitPvirgule));
            }

            try
            {
                this.Comment = comment;
                this.Name = splitPvirgule[1];
                this.Size = new Size(int.Parse(splitPvirgule[3]), int.Parse(splitPvirgule[2]));
                this.Location = new Point(
                    int.Parse(splitPvirgule[5]) + (fromCopy ? 10 : 0),
                    int.Parse(splitPvirgule[4]) + (fromCopy ? 10 : 0)
                );
                this.LevelVisible = int.Parse(splitPvirgule[7]);
                this.LevelEnabled = int.Parse(splitPvirgule[8]);
                this.Detecteur = splitPvirgule[6];

                if (splitPvirgule.Length >= 10)
                {
                    this.Visibility = splitPvirgule[9];
                }
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Les données du fichier contiennent des valeurs invalides.", nameof(splitPvirgule), ex);
            }

            return this;
        }

        public string WriteFile()
        {
            return $"INTEG;{this.Name};{this.Size.Height};{this.Size.Width};{this.Location.Y};{this.Location.X};{this.Detecteur};{this.LevelVisible};{this.LevelEnabled};{this.Visibility}";
        }
        #endregion

        #region Gestion des événements
        private void INTEG_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region Masquage des propriétés inutilisées
        [Browsable(false)]
        public new Color BackColor { get; set; }

        [Browsable(false)]
        public new Color ForeColor { get; set; }

        [Browsable(false)]
        public new Font Font { get; set; }

        [Browsable(false)]
        public new AccessibleRole AccessibleRole { get; set; }

        [Browsable(false)]
        public new string AccessibleDescription { get; set; }

        [Browsable(false)]
        public new string AccessibleName { get; set; }

        [Browsable(false)]
        public new Image BackgroundImage { get; set; }

        [Browsable(false)]
        public new ImageLayout BackgroundImageLayout { get; set; }

        [Browsable(false)]
        public new BorderStyle BorderStyle { get; set; }

        [Browsable(false)]
        public new Cursor Cursor { get; set; }

        [Browsable(false)]
        public new RightToLeft RightToLeft { get; set; }

        [Browsable(false)]
        public new bool UseWaitCursor { get; set; }

        [Browsable(false)]
        public new bool AllowDrop { get; set; }

        [Browsable(false)]
        public new AutoValidate AutoValidate { get; set; }

        [Browsable(false)]
        public new ContextMenuStrip ContextMenuStrip { get; set; }

        [Browsable(false)]
        public new bool Enabled { get; set; }

        [Browsable(false)]
        public new ImeMode ImeMode { get; set; }

        [Browsable(false)]
        public new int TabIndex { get; set; }

        [Browsable(false)]
        public new bool TabStop { get; set; }

        [Browsable(false)]
        public new bool Visible { get; set; }

        [Browsable(false)]
        public new AnchorStyles Anchor { get; set; }

        [Browsable(false)]
        public new bool AutoScroll { get; set; }

        [Browsable(false)]
        public new Size AutoScrollMargin { get; set; }

        [Browsable(false)]
        public new Size AutoScrollMinSize { get; set; }

        [Browsable(false)]
        public new bool AutoSize { get; set; }

        [Browsable(false)]
        public new AutoSizeMode AutoSizeMode { get; set; }

        [Browsable(false)]
        public new DockStyle Dock { get; set; }

        [Browsable(false)]
        public new Padding Margin { get; set; }

        [Browsable(false)]
        public new Size MaximumSize { get; set; }

        [Browsable(false)]
        public new Size MinimumSize { get; set; }

        [Browsable(false)]
        public new Padding Padding { get; set; }

        [Browsable(false)]
        public new object Tag { get; set; }

        [Browsable(false)]
        public new bool CausesValidation { get; set; } 
        #endregion

        // Propriétés supplémentaires
        public string Type => "INTEG";
        public string SType => "INTEG";

        public Type GType() => this.GetType();
    }
}