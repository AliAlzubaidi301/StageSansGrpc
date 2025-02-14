using OrthoDesigner.LIB;
using StageCode;
using StageCode.LIB;
using System.Text;

namespace OrthoDesigner.Other
{
    public class CopyAndPasteAndCut
    {
        public static string texte = string.Empty;
        public static string type = string.Empty;

        public static Control? ctrl;
        public static PictureBox? pic;
        public void TransformeToTxt(PictureBox picTmp)
        {
            if (picTmp.Controls.Count == 0) return;

            Control? childControl = picTmp.Controls[0];
            StringBuilder accumulatedText = new StringBuilder();

            switch (childControl)
            {
                case AM60 am60Control:
                    type = "AM60";
                    accumulatedText.AppendLine(am60Control.WriteFileXML());
                    break;
                case CONT1 cont1Control:
                    type = "CONT1";
                    accumulatedText.AppendLine(cont1Control.WriteFileXML());
                    break;
                case INTEG integControl:
                    type = "INTEG";
                    accumulatedText.AppendLine(integControl.WriteFileXML());
                    break;
                case OrthoAD orthoADControl:
                    type = "OrthoAD";
                    accumulatedText.AppendLine(orthoADControl.WriteFileXML());
                    break;
                case OrthoAla orthoAlaControl:
                    type = "OrthoAla";
                    accumulatedText.AppendLine(orthoAlaControl.WriteFileXML());
                    break;
                case OrthoCMDLib orthoCMDLibControl:
                    type = "OrthoCMDLib";
                    accumulatedText.AppendLine(orthoCMDLibControl.WriteFileXML());
                    break;
                case OrthoCombo orthoComboControl:
                    type = "OrthoCombo";
                    accumulatedText.AppendLine(orthoComboControl.WriteFileXML());
                    break;
                case OrthoDI orthoDIControl:
                    type = "OrthoDI";
                    accumulatedText.AppendLine(orthoDIControl.WriteFileXML());
                    break;
                case OrthoEdit orthoEditControl:
                    type = "OrthoEdit";
                    accumulatedText.AppendLine(orthoEditControl.WriteFileXML());
                    break;
                case OrthoImage orthoImageControl:
                    type = "OrthoImage";
                    accumulatedText.AppendLine(orthoImageControl.WriteFileXML());
                    break;
                case OrthoLabel orthoLabelControl:
                    type = "OrthoLabel";
                    accumulatedText.AppendLine(orthoLabelControl.WriteFileXML());
                    break;
                case OrthoPbar orthoPbarControl:
                    type = "OrthoPbar";
                    accumulatedText.AppendLine(orthoPbarControl.WriteFileXML());
                    break;
                case OrthoRel orthoRelControl:
                    type = "OrthoRel";
                    accumulatedText.AppendLine(orthoRelControl.WriteFileXML());
                    break;
                case OrthoResult orthoResultControl:
                    type = "OrthoResult";
                    accumulatedText.AppendLine(orthoResultControl.WriteFileXML());
                    break;
                case OrthoVarname orthoVarnameControl:
                    type = "OrthoVarname";
                    accumulatedText.AppendLine(orthoVarnameControl.WriteFileXML());
                    break;
                case Reticule reticuleControl:
                    type = "Reticule";
                    accumulatedText.AppendLine(reticuleControl.WriteFileXML());
                    break;
                case OrthoTabname tabNAME:
                    type = "OrthoTabname";
                    accumulatedText.AppendLine(tabNAME.WriteFileXML());
                    break;
            }

            texte = accumulatedText.ToString();

            pic = new PictureBox();
            MessageBox.Show(texte);
        }

        public void Paste(Point position)
        {
            if (string.IsNullOrEmpty(texte)) return;
            MessageBox.Show(texte);

            switch (type)
            {
                case "AM60":
                    var AM60 = new AM60();
                    ctrl = AM60.ReadFileXML(texte);
                    break;
                case "CONT1":
                    var CONT1 = new CONT1();
                    ctrl = CONT1.ReadFileXML(texte);
                    break;
                case "INTEG":
                    var INTEG = new INTEG();
                    ctrl = INTEG.ReadFileXML(texte);
                    break;
                case "OrthoAD":
                    var OrthoAD = new OrthoAD();
                    ctrl = OrthoAD.ReadFileXML(texte);
                    break;
                case "OrthoAla":
                    var OrthoAla = new OrthoAla();
                    ctrl = OrthoAla.ReadFileXML(texte);
                    break;
                case "OrthoCMDLib":
                    var OrthoCMDLib = new OrthoCMDLib();
                    ctrl = OrthoCMDLib.ReadFileXML(texte);
                    break;
                case "OrthoCombo":
                    var OrthoCombo = new OrthoCombo();
                    ctrl = OrthoCombo.ReadFileXML(texte);
                    break;
                case "OrthoDI":
                    var OrthoDI = new OrthoDI();
                    ctrl = OrthoDI.ReadFileXML(texte);
                    break;
                case "OrthoEdit":
                    var OrthoEdit = new OrthoEdit();
                    ctrl = OrthoEdit.ReadFileXML(texte);
                    break;
                case "OrthoImage":
                    var OrthoImage = new OrthoImage();
                    ctrl = OrthoImage.ReadFileXML(texte);
                    break;
                case "OrthoLabel":
                    var OrthoLabel = new OrthoLabel();
                    ctrl = OrthoLabel.ReadFileXML(texte);
                    break;
                case "OrthoPbar":
                    var OrthoPbar = new OrthoPbar();
                    ctrl = OrthoPbar.ReadFileXML(texte);
                    break;
                case "OrthoRel":
                    var OrthoRel = new OrthoRel();
                    ctrl = OrthoRel.ReadFileXML(texte);
                    break;
                case "OrthoResult":
                    var OrthoResult = new OrthoResult();
                    ctrl = OrthoResult.ReadFileXML(texte);
                    break;
                case "OrthoVarname":
                    var OrthoVarname = new OrthoVarname();
                    ctrl = OrthoVarname.ReadFileXML(texte);
                    break;
                case "Reticule":
                    var Reticule = new Reticule();
                    ctrl = Reticule.ReadFileXML(texte);
                    break;
                case "OrthoTabname":
                    ctrl = OrthoTabname.ReadFileXML(texte);
                    break;
                default:
                    return;
            }
            pic.Size = ctrl.Size;
            pic.Width += 10;
            pic.Height += 10;
            ctrl.Location = new Point(5, 5);
        }
    }
}
