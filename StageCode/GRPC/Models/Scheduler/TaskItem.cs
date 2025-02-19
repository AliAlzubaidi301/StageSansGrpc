using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Scheduler
{
    public class TaskItem
    {
        public Action RefreshDatas { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }


        //public long ReccurrenceNumber { get; set; }

        private DateTime firstOccurrrence;
        public DateTime FirstOccurrrence
        {
            get
            {
                return firstOccurrrence;
            }
            set
            {
                firstOccurrrence = value;
                if (firstOccurrrence >= LastOccurrence)
                {
                }
            }
        }

        private DateTime lastOccurrence;
        public DateTime LastOccurrence
        {
            get
            {
                return lastOccurrence;
            }
            set
            {
                if (value <= firstOccurrrence)
                {
                }
                else
                {
                    lastOccurrence = value;
                }
            }
        }

        public bool IsArchived { get; set; }

        public string Owner { get; set; }

        public DateTime LastModification { get; set; }

        public string JobName { get; set; }

    }
}
