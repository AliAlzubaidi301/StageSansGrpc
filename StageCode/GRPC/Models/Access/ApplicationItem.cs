using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Access
{
    public class ApplicationItem
    {
        public long IdApplication { get; set; }

        public string ApplicationGuid { get; set; }

        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
