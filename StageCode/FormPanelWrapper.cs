using System.ComponentModel;

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
        Control = control;
    }
}