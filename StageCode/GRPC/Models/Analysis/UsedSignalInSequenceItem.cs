using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class UsedSignalInSequenceItem
    {

        public string Title { 
            get { return ControllerId.ToString() + " - " + Name + ", durée : " + Duration.ToString()+ " seconds"; }
            set { }
        }
        public long ControllerId { get; set; }
        public double Duration { get; set; }
        public string Name { get; set; }
        public List<UsedImpuritiesItemInSequenceItem> ImpuritiesUsed { get;set;}
        public UsedSignalInSequenceItem()
        {
            ImpuritiesUsed = new List<UsedImpuritiesItemInSequenceItem>();
        }
    }
}
