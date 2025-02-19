using Orthodyne.CoreCommunicationLayer.Models.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class ResultIoOutputItem
    {
        private bool isConvertible = false;

        public const double VALUE_FOR_NOT_SET_RANGE_LOW = 0;
        public const double VALUE_FOR_NOT_SET_RANGE_HIGH = 100;
        public IoController TargetController { get; set; }
        public double LowRangeValue { get; set; }
        public double HighRangeValue { get; set; }

        public bool IsConvertible
        {
            get
            {
                return isConvertible;
            }
            set
            {
                isConvertible = value;
            }
        }

        public ResultIoOutputItem() {
            LowRangeValue = VALUE_FOR_NOT_SET_RANGE_LOW;
            HighRangeValue = VALUE_FOR_NOT_SET_RANGE_HIGH;
        }
        
        public ResultIoOutputItem CopyItem() 
        {
            ResultIoOutputItem copy = new ResultIoOutputItem()
            {
                HighRangeValue = this.HighRangeValue,
                LowRangeValue = this.LowRangeValue,
                IsConvertible = this.IsConvertible
            };
            if (this.TargetController != null)
            {
                copy.TargetController = this.TargetController.CopyItem();
            }
            return copy;
        }

        
    }
}
