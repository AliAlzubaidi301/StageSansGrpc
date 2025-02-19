using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class StatusIoStream
    {
        public long IoStreamId;                  //0 not used, -1 GeneralStatus ( for all status), >0 IoStream ID
        public bool IsStatus { get; set; }
        public string SimpleNameToDisplay { get; set; }     //Check if Simplename is Changed
    }
}
