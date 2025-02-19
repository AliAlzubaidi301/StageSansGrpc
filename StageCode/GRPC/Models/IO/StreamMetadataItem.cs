using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class StreamMetadataItem
    {
        private List<StreamMetadataPropertyItem> metadataProperties = new List<StreamMetadataPropertyItem>();
        public string StreamShortType { get; set; }
        public List<StreamMetadataPropertyItem> MetadataProperties { get { return metadataProperties; } }
    }
}
