using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace StageCode
{
    [Serializable]
    public partial class AM60 : UserControl
    {
        #region "Orthodyne Data"
        private int _LevelVisible = 0;
        private int _LevelEnabled = 0;
        private string _comment = string.Empty;
        private string _visibility = "1";
        #endregion

        #region "Control Data"
        private string _Detecteur;
        #endregion

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        #region "Read/Write on .syn file"
        public AM60 ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);

            AM60 am60Control = new AM60();

            string? type = xml.Attribute("type")?.Value;
            am60Control.Name = xml.Attribute("name")?.Value;

            // Parse the <Apparence> section
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                // Extract values from the <Apparence> section
                am60Control.BackColor = System.Drawing.Color.FromName(appearance.Element("Backcolor")?.Attribute("value")?.Value ?? "Transparent");
                am60Control.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Attribute("value")?.Value ?? "0");
                am60Control.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Attribute("value")?.Value ?? "0");
                am60Control.Detecteur = appearance.Element("Detecteur")?.Attribute("value")?.Value ?? "";
                am60Control.Visibility = appearance.Element("Visibility")?.Attribute("value")?.Value ?? "Visible";
            }

            // Return the populated AM60 object
            return am60Control;
        }
        public AM60 ReadFile(string[] splitPvirgule, string comment, string file, bool fromCopy)
        {
            this._comment = comment;
            this.Name = splitPvirgule[1];
            this.Size = new Size(int.Parse(splitPvirgule[3]), int.Parse(splitPvirgule[2]));

            if (fromCopy)
            {
                this.Location = new Point(int.Parse(splitPvirgule[5]) + 10, int.Parse(splitPvirgule[4]) + 10);
            }
            else
            {
                this.Location = new Point(int.Parse(splitPvirgule[5]), int.Parse(splitPvirgule[4]));
            }

            this.LevelVisible = int.Parse(splitPvirgule[7]);
            this.LevelEnabled = int.Parse(splitPvirgule[8]);
            this.Detecteur = splitPvirgule[6];

            if (splitPvirgule.Length >= 10)
            {
                this.Visibility = splitPvirgule[9];
            }

            return this;
        }

        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            // Début du composant spécifique
            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");

            // Section Apparence
            xmlContent.AppendLine("      <Apparence>");
            xmlContent.AppendLine($"        <Backcolor value=\"{this.BackColor.Name.ToLower()}\"/>");
            xmlContent.AppendLine($"        <LevelVisible value=\"{this.LevelVisible}\"/>");
            xmlContent.AppendLine($"        <LevelEnabled value=\"{this.LevelEnabled}\"/>");

            // Gestion de la valeur "Detecteur"
            string detecteurValue = string.IsNullOrEmpty(this.Detecteur) ? "undefined" : this.Detecteur;
            xmlContent.AppendLine($"        <Detecteur value=\"{detecteurValue}\"/>");

            // Gestion de la visibilité
            xmlContent.AppendLine($"        <Visibility value=\"{this.Visibility}\"/>");
            xmlContent.AppendLine("      </Apparence>");

            // Fermeture du composant
            xmlContent.AppendLine("    </Component>");

            // Retourner le contenu XML généré
            return xmlContent.ToString();
        }


        public string WriteFile()
        {
            return $"AM60;{this.Name};{this.Size.Height};{this.Size.Width};{this.Location.Y};{this.Location.X};{this.Detecteur};{this.LevelVisible};{this.LevelEnabled};{this.Visibility}";
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
        public new BorderStyle BorderStyle
        {
            get { return BorderStyle; }
            set { BorderStyle = value; }
        }

        [Browsable(false)]
        public new Cursor Cursor
        {
            get { return base.Cursor; }
            set { base.Cursor = value; }
        }

        [Browsable(false)]
        public new RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; }
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
            get { return base.Enabled; }
            set { base.Enabled = value; }
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
            set { base.Tag = value; }
        }

        [Browsable(false)]
        public new bool CausesValidation
        {
            get { return base.CausesValidation; }
            set { base.CausesValidation = value; }
        }
        #endregion

        public string Type
        {
            get { return "AM60"; }
        }

        public string SType
        {
            get { return "AM60"; }
        }

        public Type GType()
        {
            return this.GetType();
        }

        public AM60()
        {
            InitializeComponent();
        }

        private void AM60_Load(object sender, EventArgs e)
        {

        }
    }
}
