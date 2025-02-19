using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    [Serializable]
    public class FormulaItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsEditable { get; set; }
        public FormulaItem CopyItem() {
            FormulaItem copy = new FormulaItem()
            {
                Id = this.Id,
                Name = this.Name,
                Content = this.Content,
                IsEditable = this.IsEditable
            };
            return copy;
        }
    }
}
