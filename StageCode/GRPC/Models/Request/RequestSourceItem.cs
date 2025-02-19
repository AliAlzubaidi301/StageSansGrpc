using Orthodyne.CoreCommunicationLayer.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Request
{
    public class RequestSourceItem
    {
        private GenericPropertyContainer properties = new GenericPropertyContainer();
        public long Id { get; set; }
        public string InfoMessage { get; set; }
        public RequestActionItem Action { get; set; }
        public GenericPropertyContainer Properties { get { return properties; } set { properties = value; } }

        public RequestSourceItem CopyItem() {
            RequestSourceItem returnValue = new RequestSourceItem();
            returnValue.Id = this.Id;
            returnValue.properties = this.properties.CopyItem();
            returnValue.Action = this.Action.CopyItem();
            return returnValue;
        }
    }

}
