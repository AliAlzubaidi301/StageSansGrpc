using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Job
{
    public class VariableItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
        public bool Initialized { get; set; }
    }
}
