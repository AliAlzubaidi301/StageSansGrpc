using Orthodyne.CoreCommunicationLayer.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Request
{
    public class RequestActionItem
    {
        private GenericPropertyContainer properties = new GenericPropertyContainer();
        public string InfoMessage { get; set; }
        public GenericPropertyContainer Properties { get { return properties; } set { properties = value; } }
        public RequestActionItem CopyItem() {
            RequestActionItem returnValue = new RequestActionItem();
            returnValue.properties = this.Properties.CopyItem();
            return returnValue;
        }
    }
}
