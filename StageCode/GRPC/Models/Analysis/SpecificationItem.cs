using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class SpecificationItem
    {
        public const double VALUE_FOR_NOT_SET = -9999999;
        public double AlarmHighHighLevel { get; set; }
        public double AlarmHighLevel { get; set; }
        public double AlarmLowLowLevel { get; set; }
        public double AlarmLowLevel { get; set; }
        public double TresholdHigh { get; set; }
        public double TresholdLow { get; set; }
        public long Version { get; set; }
        public string StartApplyingDate { get; set; }
        public string IncertitudeMode { get; set; }
        public double IncertitudeValue { get; set; }
        public long Id { get; set; }
        public SpecificationItem()
        {
            AlarmHighHighLevel = VALUE_FOR_NOT_SET;
            AlarmHighLevel = VALUE_FOR_NOT_SET;
            AlarmLowLowLevel = VALUE_FOR_NOT_SET;
            AlarmLowLevel = VALUE_FOR_NOT_SET;
            TresholdHigh = VALUE_FOR_NOT_SET;
            TresholdLow = VALUE_FOR_NOT_SET;
            Version = 0;
            StartApplyingDate = "";
            IncertitudeMode = "";
            IncertitudeValue = 0;
            Id = 0;
        }

        public SpecificationItem CopyItem()
        {
            SpecificationItem copy = new SpecificationItem() { 
                Id = this.Id,
                AlarmHighHighLevel = this.AlarmHighHighLevel,
                AlarmHighLevel = this.AlarmHighLevel,
                AlarmLowLevel = this.AlarmLowLevel,
                AlarmLowLowLevel = this.AlarmLowLowLevel,
                IncertitudeMode = this.IncertitudeMode,
                IncertitudeValue = this.IncertitudeValue,
                StartApplyingDate = this.StartApplyingDate,
                TresholdHigh = this.TresholdHigh,
                TresholdLow = this.TresholdLow,
                Version = this.Version
            };
            return copy;
        }
    }
}
