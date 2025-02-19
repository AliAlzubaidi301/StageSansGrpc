using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class SequenceItem
    {
        public const string METHOD_NAME_PROPERTY_NAME = "method_name";
        public long Id { get; set; }
        public string Name { get; set; }
        public List<UsedSignalInSequence> UsedSignal { get; set; }
        public List<UsedImpuritiesItemInSequenceItem> UsedImpurities { get; }
        public List<SpecificDataItem> VersionnedDatas { get; }
        public List<TimeEventItem> TimeEvents { get; }
        public SequenceItem() {
            UsedSignal = new List<UsedSignalInSequence>();
            UsedImpurities = new List<UsedImpuritiesItemInSequenceItem>();
            VersionnedDatas = new List<SpecificDataItem>();
            TimeEvents = new List<TimeEventItem>();
        }

        public SequenceItem CopyItem() {
            SequenceItem copy = new SequenceItem() {
                Id = this.Id,
                Name = this.Name
            };
            foreach(SpecificDataItem data in VersionnedDatas) {
                copy.VersionnedDatas.Add(data.CopyItem());
            }
            foreach(UsedSignalInSequence usedSignal in UsedSignal) {
                copy.UsedSignal.Add(usedSignal.CopyItem());
            }
            foreach(UsedImpuritiesItemInSequenceItem impurity in UsedImpurities) {
                copy.UsedImpurities.Add(impurity.CopyItem());
            }
            foreach(TimeEventItem timeEvent in TimeEvents) {
                copy.TimeEvents.Add(timeEvent.CopyItem());
            }
            return copy;
        }
    }
}
