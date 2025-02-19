using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class StreamMetadataPropertyItem
    {
        public string PropertyName { get; set; }
        public string PropertyTypeInString { get; set; }
        public string PropertyValueInString { get; set; }
        public string LabelForUi { get; set; }
    }
}
