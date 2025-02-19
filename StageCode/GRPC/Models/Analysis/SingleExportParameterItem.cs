using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class SingleExportParameterItem
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public object PropertyValue { get; set; }
    }
}
