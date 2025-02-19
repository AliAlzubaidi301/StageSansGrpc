using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class IoStreamDataTableItem
    {
        private List<IoItemPropertyItem> identifierProperties = new List<IoItemPropertyItem>();
        private List<LinkItem> linkedControllers = new List<LinkItem>();
        private List<string> authorizedControllerTypes = new List<string>();
        public string Name { get; set; }
        public string Unit { get; set; }    //Append by Lbo
        // Append for Alarms
        public string ErrorMsg { get; set; }
        public int Priority { get; set; }
        public int Type { get; set; }
        public double MinValue { get; set; }    //Append by Lbo for Signal
        public double MaxValue { get; set; }    //Append by Lbo for Signal
        public long ScaleMin { get; set; }      //Append by TCO for scaling chart
        public long ScaleMax { get; set; }      //Append by TCO for scaling chart

        public int Id { get; set; }
        public List<IoItemPropertyItem> GetIdentifierProperties() { return identifierProperties; }
        public List<LinkItem> GetLinkedControllers() { return linkedControllers; }
        public List<string> GetauthorizedControllerTypes() { return authorizedControllerTypes; }
    }
}
