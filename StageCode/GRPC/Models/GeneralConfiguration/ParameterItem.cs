using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.GeneralConfiguration
{
    public class ParameterItem
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Comment { get; set; }
    }
}
