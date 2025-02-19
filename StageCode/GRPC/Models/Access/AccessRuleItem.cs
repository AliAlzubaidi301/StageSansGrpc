using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Access
{
    public class AccessRuleItem
    {
        public AccessRuleItem()
        {
            ApplyingApplicationGuid = new List<string>();
            //AccessGranted = new List<long>();
            AccessGranted = new Dictionary<long, bool>();
        }

        public long Id { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
        public bool IsEditable { get; set; }
        public List<string> ApplyingApplicationGuid { get; set; }

        public Dictionary<long,bool> AccessGranted { get; set; }

        public string TechnicalName { get; set; }
    }
}
