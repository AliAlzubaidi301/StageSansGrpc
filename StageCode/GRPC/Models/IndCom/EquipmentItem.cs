using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IndCom
{
    public class EquipmentItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IndCommTypeEnum Type { get; set; }
        public List<Item> Items { get; set; }
        public List<ParamItem> Params { get; set; }
    }
}
