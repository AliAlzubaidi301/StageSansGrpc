using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Scheduler
{
    public class CallStackItem
    {
        public long Id { get; set; }

        public long IdTask { get; set; }

        public DateTime ExecutionDate { get; set; }

        public string Job { get; set; }

        public string Status { get; set; }

    }
}
