using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    [Serializable]
    public class TimeEventItem : INotifyPropertyChanged
    {
        public long Id { get; set; }
        public long IdSequence { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
        public long IdTargetController { get; set; }
        public string Comment { get; set; }
        public string ItemTypeInString { get; set; }
        public string CreationDate { get; set; }
        public long Version { get; set; }
        public TimeEventItem() {
            Id = 0;
            CreationDate = "";
            Version = 0;
            Comment = "";
        }

        private void NotifyPropertyChanged(string property)
        {

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public TimeEventItem CopyItem() {
            TimeEventItem copy = new TimeEventItem()
            {
                Id = this.Id,
                IdSequence = this.IdSequence,
                ExecutionTime = this.ExecutionTime,
                IdTargetController = this.IdTargetController,
                Comment = this.Comment,
                ItemTypeInString = this.ItemTypeInString,
                CreationDate = this.CreationDate,
                Version = this.Version,
                Value = this.Value,
            };
            return copy;
        }
    }
}
