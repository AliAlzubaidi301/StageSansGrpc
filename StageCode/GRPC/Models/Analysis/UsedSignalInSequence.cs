using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class UsedSignalInSequence
    {
        public long ControllerId { get; set; }
        public double Duration { get; set; }
        public string ComponentName { get; set; }
        public string SignalName { get; set; }
        public List<SpecificDataItem> VersionnedDatas { get; }
        public int ForSorting { get; set; }

        public UsedSignalInSequence ()
        {
            VersionnedDatas = new List<SpecificDataItem>();
        }

        public UsedSignalInSequence CopyItem() {
            UsedSignalInSequence returnValue = new UsedSignalInSequence()
            {
                ControllerId = this.ControllerId,
                Duration = this.Duration,
                ComponentName = this.ComponentName,
                SignalName = this.SignalName
            };
            foreach (SpecificDataItem data in VersionnedDatas)
            {
                returnValue.VersionnedDatas.Add(data.CopyItem());
            }
            return returnValue;
        }
    }
}
