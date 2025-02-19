using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Access
{
    public class LinkRuleSecuredAccessItem
    {
        public long IdRule { get; set; }
        //Id user OR idGroup it's not possible to have the same id for both case as they share the same table in DB
        public long IdSecuredItem { get; set; }
        public bool IsAccessGranted { get; set; }
    }
}
