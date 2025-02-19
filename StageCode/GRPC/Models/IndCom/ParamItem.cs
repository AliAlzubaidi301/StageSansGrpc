using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IndCom
{
    public class ParamItem
    {
        public long Id { get; set; }
        public long IdEqu { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
