using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    
    public class TimeEventTemplateItem
    {
        public string TimeEventName { get; set; }
        public string ItemTypeInString { get; set; }
        public List<string> AuthorisecControllersTypeAsTarget { get; set; }
        public TimeEventTemplateItem()
        {
            AuthorisecControllersTypeAsTarget = new List<string>();
        }
    }
    
}
