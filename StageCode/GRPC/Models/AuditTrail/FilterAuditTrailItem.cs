using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.AuditTrail
{
    public class FilterAuditTrailItem
    {
        public long? Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DataType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string User { get; set; }
        public string Comment { get; set; }
        public string ComputerName { get; set; }
        public string Licence { get; set; }
    }
}
