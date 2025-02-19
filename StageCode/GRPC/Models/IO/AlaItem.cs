using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class AlaItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SimpleNameToDisplay { get; set; }
        public string ErrorMessage { get; set; }
        public string Priority { get; set; }
        public int Type{ get; set; }
        public string TypeStr { get; set; }
        public int AlarmState { get; set; }
        public string AlarmStateStr { get; set; }
        public int MainItemId { get; set; }
        public string MainItemName { get; set; }
        public string HardwareMaster { get; set; }

        public AlaItem()
        {

        }
    }
}
