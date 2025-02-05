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
using static System.Net.Mime.MediaTypeNames;

namespace StageCode.LIB
{
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

        public string WriteFile()
        {
            return $"CONT1;{Name};{Size.Height};{Size.Width};{Location.Y};{Location.X};{Detecteur};{LevelVisible};{LevelEnabled};{Visibility}";
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"<Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("  <Apparence>");
            xmlContent.AppendLine($"    <SizeHeight>{Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"    <SizeWidth>{Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"    <LocationY>{Location.Y}</LocationY>");
            xmlContent.AppendLine($"    <LocationX>{Location.X}</LocationX>");
            xmlContent.AppendLine($"    <Detecteur>{Detecteur}</Detecteur>");
            xmlContent.AppendLine($"    <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"    <LevelEnabled>{LevelEnabled}</LevelEnabled>");
            xmlContent.AppendLine($"    <Visibility>{Visibility}</Visibility>");
            xmlContent.AppendLine("  </Apparence>");
            xmlContent.AppendLine("</Component>");

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
