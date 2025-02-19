using Orthodyne.CoreCommunicationLayer.Models.Alarms;
using Orthodyne.CoreCommunicationLayer.Models.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Job
{
    [Serializable]
    public class JobItem
    {
        private List<string> brutContentLines = new List<string>();
        private List<string> errorLines = new List<string>();
        
        
        public JobItem() 
        { 
            BlockId = -1; 
        }
        public JobCurrentTaskProgressItem PreviousProgressState { get; set; }
        public JobCurrentTaskProgressItem CurrentProgressState { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsRunning { get; set; }
        public long BlockId { get; set; }
        public List<string> BrutContentLines { get { return this.brutContentLines; } }
        public List<string> ErrorLines { get { return this.errorLines; } }
        public DateTime LastEditionDate { get; set; }
        public bool IsArchive { get; set; }
        public int ErrorNumber { get; set; }
        public string Owner { get; set; }
        public string Type { get; set; }
        public bool IsJobInDB { get; set; }

        public object ErrorNumberColor 
        {
            get 
            { 
                    return Brushes.Red;
            }
        }

    }
}
