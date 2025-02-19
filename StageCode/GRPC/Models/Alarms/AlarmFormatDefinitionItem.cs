using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Alarms
{
    public class AlarmFormatDefinitionItem
    {
        private List<AlarmPropertyItem> expectedProperties = new List<AlarmPropertyItem>();
        public string AlarmType { get; set; }
        public string AlarmImplementationType { get; set; }
        public List<AlarmPropertyItem> ExpectedProperties { get { return expectedProperties; } }
    }
}
