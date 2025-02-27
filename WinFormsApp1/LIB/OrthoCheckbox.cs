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

namespace OrthoDesigner.LIB___Copier
{
    public partial class OrthoCheckbox : UserControl
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

        public OrthoCheckbox()
        {
            InitializeComponent();
        }



        private void OrthoCheckbox_Load(object sender, EventArgs e)
        {

        }

        #region "Read/Write on .syn file"
        public static OrthoCheckbox ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);
            OrthoCheckbox checkboxControl = new OrthoCheckbox();

            string? type = xml.Attribute("type")?.Value;
            checkboxControl.Name = xml.Attribute("name")?.Value;

            // Parse the <Apparence> section
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                // Extract values from the <Apparence> section
                checkboxControl.BackColor = System.Drawing.Color.FromName(appearance.Element("Backcolor")?.Attribute("value")?.Value ?? "Transparent");
                checkboxControl.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Attribute("value")?.Value ?? "0");
                checkboxControl.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Attribute("value")?.Value ?? "0");
                checkboxControl.Detecteur = appearance.Element("Detecteur")?.Attribute("value")?.Value ?? "";
                checkboxControl.Visibility = appearance.Element("Visibility")?.Attribute("value")?.Value ?? "Visible";

                // Extract Size and Location from the <Apparence> section
                checkboxControl.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "100")
                );
                checkboxControl.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")
                );
            }

            // Return the populated OrthoCheckbox object
            return checkboxControl;
        }
        public string WriteFile()
        {
            return $"OrthoCheckbox;{this.Name};{this.Size.Height};{this.Size.Width};{this.Location.Y};{this.Location.X};{this.Detecteur};{this.LevelVisible};{this.LevelEnabled};{this.Visibility}";
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

            // Ajout des informations de taille et de position
            xmlContent.AppendLine($"        <SizeWidth>{this.Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <SizeHeight>{this.Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <LocationX>{this.Location.X}</LocationX>");
            xmlContent.AppendLine($"        <LocationY>{this.Location.Y}</LocationY>");

            xmlContent.AppendLine("      </Apparence>");

            // Fermeture du composant
            xmlContent.AppendLine("    </Component>");

            // Retourner le contenu XML généré
            return xmlContent.ToString();
        }
        public OrthoCheckbox ReadFile(string[] splitPvirgule, string comment, string file, bool fromCopy)
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

        #endregion

        #region "Control Properties"
        [Category("Orthodyne"), Description("Nom du détecteur associé au checkbox")]
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

        [Category("Orthodyne"), Description("Visibility: If 0 or will be hidden, if #VarName will depend on variable value")]
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

        public string Type
        {
            get { return "Checkbox"; }
        }
    }

}
