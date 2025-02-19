using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Alarms
{
    public class AlarmOutputItem
    {
        public int IdTargetController { get; set; }
        public string FeatureName { get; set; }
        public string AlarmOutputTrigger { get; set; }
    }
}
