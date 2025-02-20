using OrthoDesigner.LIB;
using StageCode;
using StageCode.LIB;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

public class FormPanelWrapper
{
    public Panel Panel { get; set; }

    private string Name;
    [Category("Form")]
    [Description("Nom de la forme")]
    public string FormName
    {
        get { return Name; }
        set
        {
            if (Name != value)
            {
                Name = value;
                if (Panel?.Parent is Form parentForm)
                {
                    parentForm.Text = value;  
                }
            }
        }
    }

    public FormPanelWrapper(Form forme)
    {
        FormName = forme.Text;

        Panel = forme.Controls["panel1"] as Panel;  
    }
}
public class ControlPictureBoxWrapper
{
    private PictureBox PictureBox { get; set; }
    public Control Control { get; set; }

    [Description("Selectionner une réfèrence")]
    public List<CheckBoxItem> OrthoCoreItem {get;set;}

    public Point Location
    {
        get { return PictureBox?.Location ?? new Point(5, 5); }
        set
        {
            if (PictureBox != null && PictureBox.Location != value)
            {
                PictureBox.Location = value;
                PictureBox.Invalidate();
            }
        }
    }

    public ControlPictureBoxWrapper(PictureBox pictureBox, Control control)
    {
        this.PictureBox = pictureBox;
        this.Control = control;
        this.OrthoCoreItem = new List<CheckBoxItem>();

        Task.Run(() =>
        {
            RecupererDonneOrthoCore();
        });
    }

    public void RecupererDonneOrthoCore()
    {
        string texte = string.Empty;

        if (this.Control != null)
        {
            switch (this.Control.GetType().Name)
            {
                case "Chart":
                    texte = "Chart".ToUpper();
                    break;

                case "Cont1":
                    texte = "Cont1".ToUpper();
                    break;

                case "INTEG":
                    texte = "INTEG".ToUpper();
                    break;

                case "OrthoAD":
                    texte = "AD".ToUpper();
                    break;

                case "OrthoAla":
                    texte = "Ala".ToUpper();
                    break;

                case "OrthoCMDLib":
                    texte = "CMDLib".ToUpper();
                    break;

                case "OrthoCombo":
                    texte = "Combo".ToUpper();
                    break;

                case "OrthoDI":
                    texte = "DI".ToUpper();
                    break;

                case "OrthoEdit":
                    texte = "Edit".ToUpper();
                    break;

                case "OrthoImage":
                    texte = "Image".ToUpper();
                    break;

                case "OrthoLabel":
                    texte = "Label".ToUpper();
                    break;

                case "OrthoPbar":
                    texte = "Pbar".ToUpper();
                    break;

                case "OrthoRel":
                    texte = "Rel".ToUpper();
                    break;

                case "OrthoResult":
                    texte = "Result".ToUpper();
                    break;

                case "OrthoVarname":
                    texte = "Varname".ToUpper();
                    break;

                case "Reticule":
                    texte = "Reticule".ToUpper();
                    break;

                case "OrthoSTMLINES":
                    texte = "STMLINES".ToUpper();
                    break;

                case "OrthoStmLineGroupe":
                    texte = "StmLineGroupe".ToUpper();
                    break;

                default:
                    texte = "Type inconnu".ToUpper();
                    break;
            }
        }

        var liste = Forme1.AfficherContenuListeGRPCParType(texte);

    //    MessageBox.Show("Le type de contrôle sélectionné est : " + texte);

        foreach (var item in liste)
        {
            CheckBoxItem check = new CheckBoxItem();
            check.Name = item;
            OrthoCoreItem.Add(check);
        }
    }
}
public class CheckBoxItem
{
    public string Name { get; set; }

    [Category("Options")]
    [Description("Cochez cette option")]
    public bool Selected { get; set; }
}

