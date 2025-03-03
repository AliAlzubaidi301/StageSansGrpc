using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace StageCode.LIB
{
    public partial class OrthoPbar : UserControl
    {
        private int _LevelVisible = 0; // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0; // Niveau d'accès minimum pour rendre l'objet accessible
        private string _comment = ""; // Commentaire sur le contrôle
        private System.Windows.Forms.ProgressBar pbar;

        private string _value, _min, _max; // Valeur numérique ou nom d'une variable
                                           // Ajout 1.0.10 Les 9 varlink en fin de fichier 
        private string[] _VL = new string[9];
        private string _visibility = "1";

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

        public OrthoPbar()
        {
            InitializeComponent();
            pbar = ProgressBar1;
            // Ajoutez une initialisation quelconque après l'appel InitializeComponent().
            ControlUtils.RegisterControl(Label1, () => Visibility, h => VisibilityChanging += h, h => VisibilityChanged += h);
            base.Resize += OrthoPbar_Resize;
        }

        private void OrthoPbar_Load(object sender, EventArgs e)
        {

        }

        private void OrthoPbar_Resize(object sender, EventArgs e)
        {
            ProgressBar1.Size = this.Size;
            Label1.Size = new Size(this.Width / 2 - Label1.Width / 2, this.Height / 2 - Label1.Height / 2);
        }

        #region Read/Write on .syn file
        public object ReadFile(string[] splitPvirgule, string comment, string file, bool FromCopy)
        {
            this.Name = splitPvirgule[0] + "_" + splitPvirgule[1];
            this.Size = new Size(int.Parse(splitPvirgule[3]), int.Parse(splitPvirgule[2]));
            if (FromCopy)
            {
                this.Location = new Point((int)Math.Round(Double.Parse(splitPvirgule[5]) + 10d), (int)Math.Round(Double.Parse(splitPvirgule[4]) + 10d));
            }
            else
            {
                this.Location = new Point(int.Parse(splitPvirgule[5]), int.Parse(splitPvirgule[4]));
            }
            LevelVisible = int.Parse(splitPvirgule[9]);
            LevelEnabled = int.Parse(splitPvirgule[10]);
            this.comment = comment;
            ToolTips = splitPvirgule[1];
            Maximum = splitPvirgule[7];
            Minimum = splitPvirgule[6];
            Value = splitPvirgule[8];
            // Ajout 23/02/2012 Les 9 Varlink en fin de ligne 
            if (Information.UBound(splitPvirgule) > 18)
            {
                _VL[0] = splitPvirgule[11];   // StartColor
                _VL[1] = splitPvirgule[12];   // EndColor (Si = StartColor pas de dégradé)
                _VL[2] = splitPvirgule[13];   // BackColor
                _VL[3] = splitPvirgule[14];   // TextColor
                _VL[4] = splitPvirgule[15];
                _VL[5] = splitPvirgule[16];
                _VL[6] = splitPvirgule[17];
                _VL[7] = splitPvirgule[18];
                _VL[8] = splitPvirgule[19];
            }
            else
            {
                // Si pas présent set les valeurs par défault
                _VL[0] = 33023.ToString();               // Orange
                _VL[1] = 16777215.ToString();           // Dégradé --> Blanc
                _VL[2] = 16777215.ToString();            // Blanc
                _VL[3] = 0.ToString();                   // Noir
                _VL[4] = 0.ToString();
                _VL[5] = 0.ToString();
                _VL[6] = 0.ToString();
                _VL[7] = 0.ToString();
                _VL[8] = 0.ToString();

                if (splitPvirgule.Length >= 21)
                {
                    Visibility = splitPvirgule[20];
                }
            }
            return this;
        }
        public OrthoPbar ReadFileXML(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);
            OrthoPbar orthoPbarControl = new OrthoPbar();

            // Parse le type et le nom de l'objet
            orthoPbarControl.Name = xml.Attribute("name")?.Value;

            // Parse la section <Apparence>
            XElement? appearance = xml.Element("Apparence");
            if (appearance != null)
            {
                // Extraire les propriétés générales
                orthoPbarControl.ToolTips = appearance.Element("ToolTips")?.Value ?? string.Empty;
                orthoPbarControl.Size = new Size(
                    int.Parse(appearance.Element("SizeWidth")?.Value ?? "100"),
                    int.Parse(appearance.Element("SizeHeight")?.Value ?? "100")
                );
                orthoPbarControl.Location = new Point(
                    int.Parse(appearance.Element("LocationX")?.Value ?? "0"),
                    int.Parse(appearance.Element("LocationY")?.Value ?? "0")
                );

                // Extraire les valeurs de progression
                orthoPbarControl.Minimum = (appearance.Element("Minimum")?.Value ?? "0");
                orthoPbarControl.Maximum = (appearance.Element("Maximum")?.Value ?? "100");
                orthoPbarControl.Value = (appearance.Element("Value")?.Value ?? "0");

                // Extraire les niveaux
                orthoPbarControl.LevelVisible = int.Parse(appearance.Element("LevelVisible")?.Value ?? "0");
                orthoPbarControl.LevelEnabled = int.Parse(appearance.Element("LevelEnabled")?.Value ?? "0");

                // Extraire les valeurs VL
                var vlElements = appearance.Elements().Where(e => e.Name.ToString().StartsWith("VL"));
                orthoPbarControl._VL = new string[vlElements.Count()];
                foreach (var vl in vlElements)
                {
                    int index = int.Parse(vl.Name.ToString().Replace("VL", ""));
                    orthoPbarControl._VL[index] = vl.Value;
                }

                // Extraire la visibilité
                orthoPbarControl.Visibility = appearance.Element("Visibility")?.Value ?? "Visible";
            }

            return orthoPbarControl;
        }

        public string WriteFile()
        {
            string retour;
            // 23/02/2012 Ajout des 9 Varlink en fin de ligne 
            retour = "PBAR;" + ToolTips + ";" + this.Size.Height.ToString() + ";" + this.Size.Width.ToString() + ";" + this.Location.Y.ToString() + ";" + this.Location.X.ToString() + ";" + Minimum + ";" + Maximum + ";" + Value + ";" + LevelVisible.ToString() + ";" + LevelEnabled.ToString() + ";" + _VL[0] + ";" + _VL[1] + ";" + _VL[2] + ";" + _VL[3] + ";" + _VL[4] + ";" + _VL[5] + ";" + _VL[6] + ";" + _VL[7] + ";" + _VL[8] + ";" + Visibility;


            return retour;
        }
        public string WriteFileXML()
        {
            var xmlContent = new StringBuilder();

            xmlContent.AppendLine($"    <Component type=\"{this.GetType().Name}\" name=\"{this.Name}\">");
            xmlContent.AppendLine("      <Apparence>");

            // Properties
            xmlContent.AppendLine($"        <ToolTips>{ToolTips}</ToolTips>");
            xmlContent.AppendLine($"        <SizeHeight>{Size.Height}</SizeHeight>");
            xmlContent.AppendLine($"        <SizeWidth>{Size.Width}</SizeWidth>");
            xmlContent.AppendLine($"        <LocationY>{Location.Y}</LocationY>");
            xmlContent.AppendLine($"        <LocationX>{Location.X}</LocationX>");
            xmlContent.AppendLine($"        <Minimum>{Minimum}</Minimum>");
            xmlContent.AppendLine($"        <Maximum>{Maximum}</Maximum>");
            xmlContent.AppendLine($"        <Value>{Value}</Value>");
            xmlContent.AppendLine($"        <LevelVisible>{LevelVisible}</LevelVisible>");
            xmlContent.AppendLine($"        <LevelEnabled>{LevelEnabled}</LevelEnabled>");

            // _VL values
            for (int i = 0; i < _VL.Length; i++)
            {
                xmlContent.AppendLine($"        <VL{i}>{_VL[i]}</VL{i}>");
            }

            xmlContent.AppendLine($"        <Visibility>{Visibility}</Visibility>");
            xmlContent.AppendLine("      </Apparence>");
            xmlContent.AppendLine("    </Component>");

            return xmlContent.ToString();
        }

        #endregion

        #region ProgressBar Properties
        [Browsable(false)]
        public Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }
        [Browsable(false)]
        public Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }
        [Browsable(false)]
        public Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }
        #endregion

        #region Orthodyne Properties
        [Category("Orthodyne")]
        [Browsable(true)]
        [Description("Titre de la barre de progression")]
        public string ToolTips
        {
            get
            {
                return Label1.Text;
            }
            set
            {
                Label1.Text = value;
            }
        }
        [Category("Orthodyne")]
        [Browsable(true)]
        [Description("Valeur numérique ou nom d'une variable")]
        public string Value
        {
            get => _value;
            set
            {
                _value = value;

                if (int.TryParse(value, out int intValue))
                {
                    ProgressBar1.Value = intValue;
                }
                else
                {
                    ProgressBar1.Value = 0;
                }
            }
        }
        [Category("Orthodyne")]
        [Browsable(true)]
        [Description("Valeur numérique ou nom d'une variable")]
        public string Minimum
        {
            get => _min;
            set
            {
                _min = value;

                if (int.TryParse(value, out int minValue))
                {
                    ProgressBar1.Minimum = minValue;
                }
                else
                {
                    ProgressBar1.Minimum = 0;
                }
            }
        }
        [Category("Orthodyne")]
        [Browsable(true)]
        [Description("Valeur numérique ou nom d'une variable")]
        public string Maximum
        {
            get => _max;
            set
            {
                _max = value;

                if (int.TryParse(value, out int maxValue))
                {
                    ProgressBar1.Maximum = Math.Max(maxValue, ProgressBar1.Minimum); // S'assure que Maximum ≥ Minimum
                }
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
        // 23/02/2012 
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink1
        {
            get
            {
                return _VL[0];
            }
            set
            {
                _VL[0] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink2
        {
            get
            {
                return _VL[1];
            }
            set
            {
                _VL[1] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink3
        {
            get
            {
                return _VL[2];
            }
            set
            {
                _VL[2] = value;
            }
        }
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
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink8
        {
            get
            {
                return _VL[7];
            }
            set
            {
                _VL[7] = value;
            }
        }
        [Category("Not assigned")]
        [Description("Variable interne non utilisé actuellement")]
        public string VarLink9
        {
            get
            {
                return _VL[8];
            }
            set
            {
                _VL[8] = value;
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
                return AccessibleDescription;
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
                return AccessibleName;
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
                return BackgroundImage;
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
                return BackgroundImageLayout;
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
                return ContextMenuStrip;
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
                return Enabled;
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
                return ImeMode;
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
                return Tag;
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
                return "PBAR";
            }
        }

        private void ProgressBar1_Click(object sender, EventArgs e)
        {

        }

        public string SType
        {
            get
            {
                return "PBAR";
            }
        }

        public Type GType()
        {
            return GetType();
        }
    }

}
