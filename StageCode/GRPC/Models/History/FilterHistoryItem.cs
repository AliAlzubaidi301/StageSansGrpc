using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.History
{
    public class FilterHistoryItem
    {
        public long? Id { get; set; }

        public string MessageType { get; set; }

        public string Content { get; set; }

        public string User { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
