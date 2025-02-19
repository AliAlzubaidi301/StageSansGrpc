using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class IoItemPropertyItem
    {
        private List<string> authorizedValues = new List<string>();
        public IoItemPropertyItem()
        {
            IsEditable = false;
        }
        public string PropertyName { get; set; }
        public string DisplayText { get; set; }
        public Type PropertyType { get; set; }
        public bool IsEditable { get; set; }
        public string PropertyValueInString { get; set; }
        public List<string> AuthorizedValues { get { return authorizedValues; } }
        public IoItemPropertyItem CopyItem() {
            IoItemPropertyItem copy = new IoItemPropertyItem() { 
                DisplayText = this.DisplayText,
                IsEditable = this.IsEditable,
                PropertyName = this.PropertyName,
                PropertyValueInString = this.PropertyValueInString,
                PropertyType = this.PropertyType                
            };
            copy.AuthorizedValues.AddRange(this.AuthorizedValues);
            return copy;
        }
    }
}

