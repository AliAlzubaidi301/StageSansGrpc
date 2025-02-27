﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace StageCode
{
    public sealed partial class INTEG : UserControl
    {
        #region "Orthodyn Data"
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private string _comment = ""; // Commentaire sur le contrôle
        private string _visibility = "1";
        #endregion

        #region "Control Data"
        private string _Detecteur;
        #endregion

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public INTEG()
        {

            InitializeComponent();

            ControlUtils.RegisterControl(this, () => Visibility,
                                (h) => VisibilityChanging += h,
                                (h) => VisibilityChanged += h);
        }

        #region "Read/Write on .syn file"

        public INTEG ReadFile(string[] splitPvirgule, string comment, string file, bool FromCopy)
        {
            this._comment = comment;
            this.Name = splitPvirgule[1];
            this.Size = new Size(int.Parse(splitPvirgule[3]), int.Parse(splitPvirgule[2]));
            this.Location = FromCopy ? new Point(int.Parse(splitPvirgule[5]) + 10, int.Parse(splitPvirgule[4]) + 10) : new Point(int.Parse(splitPvirgule[5]), int.Parse(splitPvirgule[4]));
            this.LevelVisible = int.Parse(splitPvirgule[7]);
            this.LevelEnabled = int.Parse(splitPvirgule[8]);
            this.Detecteur = splitPvirgule[6];
            if (splitPvirgule.Length >= 10)
            {
                this.Visibility = splitPvirgule[9];
            }
            return this;
        }
        public static INTEG ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);

            // Création de l'objet INTEG
            INTEG integControl = new INTEG();

            // Récupérer les attributs du composant
            integControl.Name = xml.Attribute("name")?.Value;

            // Récupérer la section <Apparence>
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                // Extraire et assigner les propriétés de l'Apparence
                integControl.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),   // Valeur par défaut 100 si manquante
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "100")  // Valeur par défaut 100 si manquante
                );

                integControl.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),   // Valeur par défaut 0 si manquante
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")    // Valeur par défaut 0 si manquante
                );

                integControl.Detecteur = appearance.Element("Detecteur")?.Value ?? ""; // Valeur par défaut vide
                integControl.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Value ?? "0");  // Valeur par défaut 0
                integControl.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Value ?? "0");  // Valeur par défaut 0
                integControl.Visibility = appearance.Element("Visibility")?.Value ?? "Visible";  // Valeur par défaut "Visible"
            }

            // Retourner l'objet INTEG avec les valeurs lues du XML
            return integControl;
        }

        public string WriteFile()
        {
            return "INTEG;" + this.Name + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() +
                   ";" + this.Detecteur + ";" + this.LevelVisible.ToString() + ";" + this.LevelEnabled.ToString() + ";" + this.Visibility;
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            // Début du composant spécifique
            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");

            // Section Apparence du composant
            xmlContent.AppendLine("      <Apparence>");
            xmlContent.AppendLine($"        <SizeHeight>{Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <SizeWidth>{Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <LocationY>{Location.Y}</LocationY>");
            xmlContent.AppendLine($"        <LocationX>{Location.X}</LocationX>");
            xmlContent.AppendLine($"        <Detecteur>{Detecteur}</Detecteur>");
            xmlContent.AppendLine($"        <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"        <LevelEnabled>{LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"        <Visibility>{Visibility}</Visibility>");
            xmlContent.AppendLine("      </Apparence>");

            // Fermeture du composant
            xmlContent.AppendLine("    </Component>");

            // Retourner le contenu XML généré
            return xmlContent.ToString();
        }


        #endregion

        #region "Control Properties"
        [Category("Orthodyne"), Description("Nom du détecteur associé au graph")]
        public string Detecteur
        {
            get { return _Detecteur; }
            set { _Detecteur = value; }
        }
        #endregion

        #region "Orthodyne Properties"
        [Category("Orthodyne"), Browsable(false), Description("Commentaire sur l'objet")]
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
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

        [Category("Orthodyne"), Description("If 0 or will be hidden, if #VarName will depend on variable value")]
        public string Visibility
        {
            get { return _visibility; }
            set
            {
                VisibilityChanging?.Invoke(this, EventArgs.Empty);
                _visibility = string.IsNullOrEmpty(value) ? "1" : value;
                VisibilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region "Hiding useless Properties"
        [Browsable(false)]
        public new Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Browsable(false)]
        public new Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        [Browsable(false)]
        public new Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        [Browsable(false)]
        public new AccessibleRole AccessibleRole
        {
            get { return base.AccessibleRole; }
            set { base.AccessibleRole = value; }
        }

        [Browsable(false)]
        public new string AccessibleDescription
        {
            get { return AccessibleDescription; }
            set { base.AccessibleDescription = value; }
        }

        [Browsable(false)]
        public new string AccessibleName
        {
            get { return AccessibleName; }
            set { AccessibleName = value; }
        }

        [Browsable(false)]
        public new Image BackgroundImage
        {
            get { return BackgroundImage; }
            set { BackgroundImage = value; }
        }

        [Browsable(false)]
        public new ImageLayout BackgroundImageLayout
        {
            get { return BackgroundImageLayout; }
            set { BackgroundImageLayout = value; }
        }

        [Browsable(false)]
        public new BorderStyle BorderStyle
        {
            get { return BorderStyle; }
            set { BorderStyle = value; }
        }

        [Browsable(false)]
        public new Cursor Cursor
        {
            get { return Cursor; }
            set { Cursor = value; }
        }

        [Browsable(false)]
        public new RightToLeft RightToLeft
        {
            get { return RightToLeft; }
            set { RightToLeft = value; }
        }

        [Browsable(false)]
        public new bool UseWaitCursor
        {
            get { return base.UseWaitCursor; }
            set { base.UseWaitCursor = value; }
        }

        [Browsable(false)]
        public new bool AllowDrop
        {
            get { return base.AllowDrop; }
            set { base.AllowDrop = value; }
        }

        [Browsable(false)]
        public new AutoValidate AutoValidate
        {
            get { return base.AutoValidate; }
            set { base.AutoValidate = value; }
        }

        [Browsable(false)]
        public new ContextMenuStrip ContextMenuStrip
        {
            get { return ContextMenuStrip; }
            set { ContextMenuStrip = value; }
        }

        [Browsable(false)]
        public new bool Enabled
        {
            get { return Enabled; }
            set { Enabled = value; }
        }

        [Browsable(false)]
        public new ImeMode ImeMode
        {
            get { return base.ImeMode; }
            set { base.ImeMode = value; }
        }

        [Browsable(false)]
        public new int TabIndex
        {
            get { return base.TabIndex; }
            set { base.TabIndex = value; }
        }

        [Browsable(false)]
        public new bool TabStop
        {
            get { return base.TabStop; }
            set { base.TabStop = value; }
        }

        [Browsable(false)]
        public new bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        [Browsable(false)]
        public new AnchorStyles Anchor
        {
            get { return base.Anchor; }
            set { base.Anchor = value; }
        }

        [Browsable(false)]
        public new bool AutoScroll
        {
            get { return base.AutoScroll; }
            set { base.AutoScroll = value; }
        }

        [Browsable(false)]
        public new Size AutoScrollMargin
        {
            get { return base.AutoScrollMargin; }
            set { base.AutoScrollMargin = value; }
        }

        [Browsable(false)]
        public new Size AutoScrollMinSize
        {
            get { return base.AutoScrollMinSize; }
            set { base.AutoScrollMinSize = value; }
        }

        [Browsable(false)]
        public new bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        [Browsable(false)]
        public new AutoSizeMode AutoSizeMode
        {
            get { return base.AutoSizeMode; }
            set { base.AutoSizeMode = value; }
        }

        [Browsable(false)]
        public new DockStyle Dock
        {
            get { return base.Dock; }
            set { base.Dock = value; }
        }

        [Browsable(false)]
        public new Padding Margin
        {
            get { return base.Margin; }
            set { base.Margin = value; }
        }

        [Browsable(false)]
        public new Size MaximumSize
        {
            get { return base.MaximumSize; }
            set { base.MaximumSize = value; }
        }

        [Browsable(false)]
        public new Size MinimumSize
        {
            get { return base.MinimumSize; }
            set { base.MinimumSize = value; }
        }

        [Browsable(false)]
        public new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        [Browsable(false)]
        public new ControlBindingsCollection DataBindings
        {
            get { return base.DataBindings; }
        }

        [Browsable(false)]
        public new object Tag
        {
            get { return Tag; }
            set { Tag = value; }
        }

        [Browsable(false)]
        public new bool CausesValidation
        {
            get { return base.CausesValidation; }
            set { base.CausesValidation = value; }
        }
        #endregion

        public string Type => "INTEG";

        public string SType => "INTEG";

        public Type GType() => this.GetType();

        private void INTEG_Load(object sender, EventArgs e)
        {
            // Logique de chargement
        }
    }
}