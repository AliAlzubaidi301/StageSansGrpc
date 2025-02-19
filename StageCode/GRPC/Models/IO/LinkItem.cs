using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class LinkItem
    {
        public int IdController { get; set; }
        public int IdStream { get; set; }
        public int IdDataTableItem { get; set; }

        public string ComponentType { get; set; }
    }
}
