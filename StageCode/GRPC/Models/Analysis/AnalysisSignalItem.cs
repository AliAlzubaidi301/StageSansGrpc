using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class AnalysisSignalItem
    {
        public AcquisitionSignal[] Values { get; set; }
        public AcquisitionSignal[] Derivees { get; set; }
        public long ControllerId { get; set; }
        public long SignalObjectId { get; set; }
        public double UsageDuration { get;set;}
        public double FirstOffsetValue { get; set; }
        public double LastInsertedOffset { get; set; }
        public int LastInsertedIndex { get; set; }
        public int LastInsertedIndexInChart { get; set; }
        public int RemainingIndex { get; set; }
        public string Name {  get; set; }
        public string SimpleNameToDisplay { get; set; }
        public List<SpecificDataItem> VersionnedDatas { get; }
        public double MinScale { get; set; }
        public double MaxScale { get; set; }

        public AnalysisSignalItem()
        {
            VersionnedDatas = new List<SpecificDataItem>();
        }

        public AnalysisSignalItem CopyItem() 
        {
            AnalysisSignalItem copy = new AnalysisSignalItem() {
                ControllerId = this.ControllerId,
                UsageDuration = this.UsageDuration,
                FirstOffsetValue = this.FirstOffsetValue,
                LastInsertedIndex = this.LastInsertedIndex,
                LastInsertedIndexInChart = this.LastInsertedIndexInChart,
                LastInsertedOffset = this.LastInsertedOffset,
                RemainingIndex = this.RemainingIndex,
                Values = this.Values,
                Derivees = this.Derivees,
                Name = this.Name,
                SimpleNameToDisplay = this.SimpleNameToDisplay,
                SignalObjectId = this.SignalObjectId,
            };

            foreach (SpecificDataItem data in VersionnedDatas)
            {
                copy.VersionnedDatas.Add(data.CopyItem());
            }
            return copy;
        }
    }
}
