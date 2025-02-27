using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace StageCode.LIB
{
    [Serializable]
    public partial class CONT1 : UserControl
    {
        private int _levelVisible = 0;
        private int _levelEnabled = 0;
        private string _comment = "";
        private string _visibility = "";
        private string _detecteur;

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public CONT1()
        {
            InitializeComponent();
            ControlUtils.RegisterControl(this, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
        }

        public CONT1 ReadFile(string[] splitPvirgule, string comment, string file, bool fromCopy)
        {
            _comment = comment;
            Name = splitPvirgule[1];
            Size = new Size(int.Parse(splitPvirgule[3]), int.Parse(splitPvirgule[2]));
            Location = fromCopy
                ? new Point(int.Parse(splitPvirgule[5]) + 10, int.Parse(splitPvirgule[4]) + 10)
                : new Point(int.Parse(splitPvirgule[5]), int.Parse(splitPvirgule[4]));
            LevelVisible = int.Parse(splitPvirgule[7]);
            LevelEnabled = int.Parse(splitPvirgule[8]);
            Detecteur = splitPvirgule[6];
            if (splitPvirgule.Length >= 10)
            {
                Visibility = splitPvirgule[9];
            }
            return this;
        }

        // Méthode pour lire un fichier XML et retourner un objet Component
        public static CONT1 ReadFileXML(string xmlText)
        {
            // Charger le XML dans un XElement
            XElement xml = XElement.Parse(xmlText);

            // Créer une nouvelle instance de Component
            CONT1 component = new CONT1();

            // Extraire les attributs du composant (type et name)
            string? type = xml.Attribute("type")?.Value;  // Le type, ici on ne l'utilise pas mais tu peux l'afficher si nécessaire
            component.Name = xml.Attribute("name")?.Value;

            // Parser la section <Apparence>
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                // Extraire les valeurs de la section <Apparence>
                component.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "0"),
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "0")
                );

                component.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")
                );

                component.Detecteur = appearance.Element("Detecteur")?.Value ?? "";
                component.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Value ?? "0");
                component.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Value ?? "0");
                component.Visibility = appearance.Element("Visibility")?.Value ?? "Visible";
            }

            // Retourner l'objet Component reconstruit
            return component;
        }
   

        public string WriteFile()
        {
            return $"CONT1;{Name};{Size.Height};{Size.Width};{Location.Y};{Location.X};{Detecteur};{LevelVisible};{LevelEnabled};{Visibility}";
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            // Début du composant spécifique
            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");

            // Section Apparence
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


        [Category("Orthodyne"), Description("Nom du détecteur associé au graph")]
        public string Detecteur
        {
            get => _detecteur;
            set => _detecteur = value;
        }

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

        public string Type => "CONT1";
        public string SType => "CONT1";

        public Type GType() => GetType();


        private void Cont1_Load(object sender, EventArgs e)
        {

        }
    }
}
