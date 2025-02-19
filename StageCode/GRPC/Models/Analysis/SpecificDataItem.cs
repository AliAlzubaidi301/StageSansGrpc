using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    [Serializable]
    public class SpecificDataItem : INotifyPropertyChanged
    {
        public long Id { get; set; }

        private string dataName;
        public string DataName
        {
            get
            {
                return dataName;
            }
            set
            {
                dataName = value;
                //TCO 28/03/2023:OS-102
                switch (dataName)
                {
                    case "span_value":
                        ForSorting = 2;
                        break;
                    case "span_signal":
                        ForSorting = 1;
                        break;
                    case "zero_value":
                        ForSorting = 4;
                        break;
                    case "zero_signal":
                        ForSorting = 3;
                        break;
                    case "drift":
                        ForSorting = 5;
                        break;
                    case "integration_stabilisation_time":
                        ForSorting = 6;
                        break;
                    case "minimum_area":
                        ForSorting = 7;
                        break;
                    case "saturation_level":
                        ForSorting = 8;
                        break;
                    case "slope":
                        ForSorting = 9;
                        break;
                    case "stability_time":
                        ForSorting = 10;
                        break;
                    case "usage_time":
                        ForSorting = 11;
                        break;
                    case "usage_duration":
                        ForSorting = 12;
                        break;

                    case "startThreshold":
                        ForSorting = 50;
                        break;
                    case "stopThreshold":
                        ForSorting = 51;
                        break;
                    case "driftThreshold":
                        ForSorting = 52;
                        break;
                    case "duration":
                        ForSorting = 53;
                        break;
                    case "height":
                        ForSorting = 54;
                        break;
                    case "saturationHigh":
                        ForSorting = 55;
                        break;
                    case "saturationLow":
                        ForSorting = 56;
                        break;
                    case "stabilisation":
                        ForSorting = 57;
                        break;
                    case "next_calibration_brut_area":
                        ForSorting = 60;
                        break;
                    case "next_calibration_value":
                        ForSorting = 61;
                        break;
                    case "next_span_value":
                        ForSorting = 72;
                        break;
                    case "next_span_signal":
                        ForSorting = 71;
                        break;
                    case "next_zero_value":
                        ForSorting = 74;
                        break;
                    case "next_zero_signal":
                        ForSorting = 73;
                        break;
                    default:
                        ForSorting = 99;
                        break;
                }
            }
        }

        public int ForSorting { get; set; }

        public string LabelForHmi { get; set; }
        public Type DataType { get; set; }
        public string[] DataValueInString { get; set; }
        public string[] OriginalDataValues { get; set; }
        public string ValueInString
        {
            get { return DataValueInString[0]; }
            set
            {
                DataValueInString[0] = value;
                NotifyPropertyChanged(nameof(ValueInString));
            }
        }
        public string OriginalValueInString
        {
            get { return OriginalDataValues[0]; }
        }
        public bool IsCalibrationValue { get; set; }
        public long Version { get; set; }
        public DateTime CreationDate { get; set; }

        private void NotifyPropertyChanged(string property)
        {

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public SpecificDataItem CopyItem()
        {
            SpecificDataItem copy = new SpecificDataItem()
            {
                Id = this.Id,
                CreationDate = this.CreationDate,
                DataName = this.DataName,
                DataType = this.DataType,
                DataValueInString = this.DataValueInString,
                IsCalibrationValue = this.IsCalibrationValue,
                Version = this.Version,
                OriginalDataValues = this.OriginalDataValues,
                LabelForHmi = this.LabelForHmi
            };
            return copy;
        }
    }
}
