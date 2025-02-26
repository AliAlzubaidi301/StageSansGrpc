using CodeExceptionManager.Model.Objects;
using Google.Protobuf.Collections;
using IIOManager;
using Orthodyne.CoreCommunicationLayer.Controllers;
using Orthodyne.CoreCommunicationLayer.Models.IO;
using Orthodyne.CoreCommunicationLayer.Services;
using StageCode;
using StageCode.LIB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

public class ControlPictureBoxWrapper
{

    [Browsable(false)]
    public PictureBox PictureBox { get; }
    public Control Control { get; }
    public const string a = "v";

    [Browsable(true), Description("Sélectionner une référence"), Editor(typeof(CheckBoxListEditor), typeof(UITypeEditor)), TypeConverter(typeof(CheckBoxItemListConverter))]
    [DisplayName("Donnée OrthoCoeur")]
    public List<CheckBoxItem> OrthoCoreItem { get; } = new();

    [Browsable(true), Description("Sélectionner un stream"), Editor(typeof(CheckBoxListEditor), typeof(UITypeEditor)), TypeConverter(typeof(CheckBoxItemListConverter))]
    [DisplayName("Donnée Stream")]
    public List<CheckBoxItem> Stream { get; } = new();

    public Point Location
    {
        get => PictureBox?.Location ?? new Point(5, 5);
        set { if (PictureBox != null) PictureBox.Location = value; }
    }

    public ControlPictureBoxWrapper(PictureBox pictureBox, Control control)
    {
        PictureBox = pictureBox ?? throw new ArgumentNullException(nameof(pictureBox));
        Control = control ?? throw new ArgumentNullException(nameof(control));

        if(Forme1.ioControllers.Count >0)
        OrthoCORE();

        if(Forme1.listeStrem.Count >0)
        AddStreamsToCheckBoxList();
    }

    public void OrthoCORE()
    {
        if (Forme1.ioControllers.Count == 0) return;

        string texte = Control?.GetType().Name switch
        {
            "Chart" => "CHART",
            "Cont1" => "CONT1",
            "INTEG" => "INTEG",
            "OrthoAD" => "AD",
            "OrthoAla" => "ALA",
            "OrthoCMDLib" => "CMDLIB",
            "OrthoCombo" => "COMBO",
            "OrthoDI" => "DI",
            "OrthoEdit" => "EDIT",
            "OrthoImage" => "IMAGE",
            "OrthoLabel" => "LABEL",
            "OrthoPbar" => "PBAR",
            "OrthoRel" => "REL",
            "OrthoResult" => "RESULT",
            "OrthoVarname" => "VARNAME",
            "Reticule" => "RETICULE",
            "OrthoSTMLINES" => "STMLINES",
            "OrthoStmLineGroupe" => "STMLINEGROUPE",
            _ => "TYPE INCONNU"
        };

        foreach (var tmp in TrierOrthoCoeurParGrpc(texte))
        {
            var checkbox = new CheckBoxItem { Name = tmp, Selected = false };
           
            OrthoCoreItem.Add(checkbox);
        }
    }
    public void AddStreamsToCheckBoxList()
    {
        if(Forme1.listeStrem.Count == 0)
        {
            return;
        }
        var module = new ModuleIoControllerOrthoDesigner(new ModuleIoRemoteMethodInvocationService(""),new GeneralController());

        module.ioStreams = Forme1.listeStrem;

        List<IoStream> targetStream = module.GetIoStreams();

        foreach(IoStream stream in targetStream)
        {
            if(!stream.IsArchive)
            {
                CheckBoxItem box = new CheckBoxItem();
                box.Name = stream.ShortType;
                box.Selected = false;

                this.Stream.Add(box);
            }
        }
        //module.Stream
    }

    public void ShowAllStreamsDataTable()
    {
        try
        {
            // Construire un message avec les données de tous les streams
            StringBuilder message = new StringBuilder();
            message.AppendLine("Données de tous les Streams :");
            message.AppendLine();

            // Récupérer les streams définis via GetDefinedStreams
            IIOManager.GetDefinedStreamsOutput definedStreamsOutput = new ModuleIoRemoteMethodInvocationService("").GetDefinedStreams();

            // Parcourir chaque stream défini
            foreach (StreamElement stream in definedStreamsOutput.Streams)
            {
                int streamId = Convert.ToInt32(stream.Identifier);

                // Récupérer les données du stream via GetStreamDataTable
                RepeatedField<StreamDataTableElement> streamDataTable =
                    new ModuleIoRemoteMethodInvocationService("").GetStreamDataTable(streamId).DataTable;

                message.AppendLine($"Stream ID : {streamId}");
                message.AppendLine($"Nom du Stream : {stream.ComponentType}");
                message.AppendLine(stream.ShortTypeName);
                message.AppendLine("Données du Stream :");

                // Parcourir chaque élément de la table de données du stream
                foreach (IIOManager.StreamDataTableElement dataTableItem in streamDataTable)
                {
                    message.AppendLine($"- Nom : {dataTableItem.DataName}");
                    message.AppendLine($"  Unité : {dataTableItem.Unit}");
                    message.AppendLine($"  Message d'erreur : {dataTableItem.Errormsg}");
                    message.AppendLine($"  Priorité : {dataTableItem.Priority}");
                    message.AppendLine($"  Type : {dataTableItem.Type}");
                    message.AppendLine($"  ID : {dataTableItem.Id}");
                    message.AppendLine($"  Valeur Min : {dataTableItem.Minvalue}");
                    message.AppendLine($"  Valeur Max : {dataTableItem.Maxvalue}");
                    message.AppendLine();
                }

                message.AppendLine("--------------------------------------------------");
            }

            // Afficher le message dans une MessageBox
            MessageBox.Show(message.ToString(), "Données de tous les Streams", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            // Loguer l'exception si une erreur se produit
            new LoggedException(
                Assembly.GetExecutingAssembly().GetName().Name,
                Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                this.GetType().Name,
                MethodBase.GetCurrentMethod().Name,
                ex.Message,
                ex.StackTrace
            );
        }
    }

    public List<string> TrierOrthoCoeurParGrpc(string typeFiltre)
    {
        List<string> listes = new List<string>();

        var liste = Forme1.ioControllers.Keys.ToList();

        foreach (var tmpKey in liste)
        {
            var controller = Forme1.ioControllers[tmpKey];
            string type = "";

            for (int i = 0; i < controller.FullName.Length; i++)
            {
                if (controller.FullName[i] == '(')
                {
                    for (int j = i + 1; j < controller.FullName.Length; j++)
                    {
                        if (controller.FullName[j] == ')')
                            break;
                        type += controller.FullName[j];
                    }
                    break;
                }
            }

            if (type == typeFiltre)
            {
                listes.Add(controller.SimpleNameNewValue); 
            }
        }

        return listes;
    }

    public void AfficherSelection()
    {
        var elementsSelectionnes = OrthoCoreItem
            .Where(item => item.Selected)
            .Select(item => item.Name)
            .ToList();

        if (elementsSelectionnes.Count > 0)
        {
            // Affiche les éléments sélectionnés dans un MessageBox
          //  MessageBox.Show(string.Join("\n", elementsSelectionnes), "Éléments sélectionnés", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Modifie le champ SimpleName dans le control
            if (Control is OrthoRel controlWrapper)
            {
                controlWrapper.SimpleNam = "";
                // Met à jour le champ SimpleName avec les éléments sélectionnés
                foreach (var element in elementsSelectionnes)
                {

                    controlWrapper.SimpleNam += element + ",";
                }
            }
            // Modifie le champ SimpleName dans le control
            if (Control is OrthoDI c2)
            {
                c2.SimpleName = "";

                // Met à jour le champ SimpleName avec les éléments sélectionnés
                foreach (var element in elementsSelectionnes)
                {
                    c2.SimpleName += element + ",";
                }
            }
            // Modifie le champ SimpleName dans le control
            if (Control is OrthoAD c3)
            {
                c3.SimpleName = "";

                // Met à jour le champ SimpleName avec les éléments sélectionnés
                foreach (var element in elementsSelectionnes)
                {
                    c3.SimpleName += element + ",";
                }
            }
        }
    }
}

public class CheckBoxItem
{
    public static string A = "Bonjour";

    [DisplayName(nameof(A))]
    public string Name { get; set; }

    [Category("Options"), Description("Cochez cette option")]
    public bool Selected { get; set; }

    public override string ToString() => Name; // Permet l'affichage du Name directement
}

// ✅ Éditeur de liste de CheckBoxItem

public class CheckBoxItemListConverter : ExpandableObjectConverter
{
    public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
        if (value is List<CheckBoxItem> items)
        {
            return new PropertyDescriptorCollection(
                items.Select((item, i) => new CheckBoxItemPropertyDescriptor(item, i)).ToArray()
            );
        }
        return base.GetProperties(context, value, attributes);
    }
}

// ✅ Propriétés des éléments dans le PropertyGrid
public class CheckBoxItemPropertyDescriptor : PropertyDescriptor
{
    private readonly CheckBoxItem item;

    public CheckBoxItemPropertyDescriptor(CheckBoxItem item, int index) : base(item.Name, null)
    {
        this.item = item;
    }

    public override Type ComponentType => typeof(CheckBoxItem);
    public override bool IsReadOnly => false;
    public override Type PropertyType => typeof(bool);
    public override bool CanResetValue(object component) => false;
    public override object GetValue(object component) => item.Selected;
    public override void SetValue(object component, object value) => item.Selected = (bool)value;
    public override void ResetValue(object component) { }
    public override bool ShouldSerializeValue(object component) => false;
}

// ✅ Classe CheckBoxItem
public class CheckBoxListEditor : UITypeEditor
{
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
        if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService && value is List<CheckBoxItem> items)
        {
            var checkedListBox = new CheckedListBox { BorderStyle = BorderStyle.None, CheckOnClick = true };
            foreach (var item in items) checkedListBox.Items.Add(item, item.Selected);
            checkedListBox.ItemCheck += (s, e) => items[e.Index].Selected = e.NewValue == CheckState.Checked;
            editorService.DropDownControl(checkedListBox);
        }
        return value;
    }
}

public class FormPanelWrapper
{
    public Panel Panel { get; set; }  // Propriété représentant un panneau (panel)

    private string name;  // Variable pour stocker le nom de la forme

    // Propriété avec un attribut pour définir la description et la catégorie de la propriété
    [Category("Form")]
    [Description("Nom de la forme")]
    public string FormName
    {
        get { return name; }  // Renvoie le nom de la forme
        set
        {
            if (name != value)  // Si la valeur du nom change
            {
                name = value;  // Modifie le nom de la forme
                if (Panel?.Parent is Form parentForm)  // Si le parent du panneau est un formulaire
                {
                    parentForm.Text = value;  // Modifie le titre du formulaire
                }
            }
        }
    }

    // Constructeur qui prend un formulaire en paramètre et initialise le nom et le panneau
    public FormPanelWrapper(Form forme)
    {
        FormName = forme.Text;  // Initialise le nom de la forme avec le texte du formulaire
        Panel = forme.Controls["panel1"] as Panel;  // Récupère le panneau nommé "panel1" du formulaire
    }
}