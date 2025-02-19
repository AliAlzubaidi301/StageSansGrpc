using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class AnalysisResultVariableItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public AnalysisResultVariableItem() 
        { 
        
        }

        public AnalysisResultVariableItem CopyItem()
        {
            AnalysisResultVariableItem copy = new AnalysisResultVariableItem()
            {
                Id = this.Id,
                Name = this.Name,
                Value = this.Value
            };
            return copy;
        }
    }
}
