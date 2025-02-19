using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Alarms
{
    public class AlarmPropertyItem
    {
        public string PropertyName { get; set; }
        public Type PropertyValueType { get; set; }
        public string LabelForHMI { get; set; }
        public object PropertyValue { get; set; }
    }
}
