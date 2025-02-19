using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Alarms
{
    public class AlarmItem
    {
        private List<AlarmOutputItem> alarmOutputs = new List<AlarmOutputItem>();
        private Dictionary<string, AlarmPropertyItem> properties = new Dictionary<string, AlarmPropertyItem>();
        public long Id { get; set; }
        public double StabilisationTime { get; set; }
        public string Label { get; set; }
        public string AlarmImplementationType { get; set; }
        public string AlarmType { get; set; }
        public bool Status { get; set; }
        public bool IsInverted { get; set; }
        public bool ToDelete { get; set; }
        public List<AlarmOutputItem> AlarmOutputs { get { return alarmOutputs; } }
        public Dictionary<string, AlarmPropertyItem> Properties { get { return properties; } }
        public string StreamSimpleNameToDisplay { get; set; }
    }
}
