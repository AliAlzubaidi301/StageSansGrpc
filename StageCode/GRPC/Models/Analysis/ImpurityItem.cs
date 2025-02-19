using Orthodyne.CoreCommunicationLayer.Models.Gas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    [Serializable]
    public class ImpurityItem
    {
        public const string SELECTED_FORMULA_PROPERTY_NAME = "selected_formula";
        public const string BEGIN_TIME_PROPERTY_NAME = "usage_time";
        public const string USAGE_DURATION_PROPERTY_NAME = "usage_duration";
        public const string CALIBRATION_BRUT_AREA = "calibration_brut_area";
        public const string CALIBRATION_VALUE = "calibration_value";
        public const string NEXT_CALIBRATION_VALUE = "next_calibration_value";
        public const string NEXT_CALIBRATION_BRUT_AREA = "next_calibration_brut_area";

        public const string SPAN_SIGNAL = "span_signal";
        public const string SPAN_VALUE = "span_value";
        public const string ZERO_SIGNAL = "zero_signal";
        public const string ZERO_VALUE = "zero_value";

        public long SignalObjectId { get; set; }
        //public UnitEnum Unit { get; set; }        
        public ImpurityUnitItem Unit { get; set; }
        //public GazNameEnum GazName { get; set; }
        public string GazName { get; set; }
        public string Color { get; set; }
        public string IdentifierName { get; set; }        
        public long Id { get; set; }        
        public List<SpecificDataItem> SpecificData { get; set; }
        public List<FormulaItem> Formulas { get; set; }
        public bool IsLinkedToResult { get; set; }
        public FormulaItem SelectedFormula 
        { 
            get
            {
                if (SpecificData.Where(x => x.DataName == SELECTED_FORMULA_PROPERTY_NAME).Count() == 1)
                {
                    return Formulas.Where(x => x.Name == SpecificData.Where(y => y.DataName == SELECTED_FORMULA_PROPERTY_NAME).FirstOrDefault().DataValueInString[0]).FirstOrDefault();
                }
                else
                {
                    return null;
                }
            } 
        }
        
        public ImpurityItem()
        {
            Id = 0;
            SignalObjectId = 0;
            IdentifierName = "";
            SpecificData = new List<SpecificDataItem>();
            Formulas = new List<FormulaItem>();
        }
        
    }

}

