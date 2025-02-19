using System;

namespace Orthodyne.CoreCommunicationLayer.Models.Job
{
    [Serializable]
    public class JobCurrentTaskProgressItem
    {
        public double Progress { get; set; }
        public string AdditionalData { get; set; }
        public bool IsCompleted { get; set; }
        public long IdLongFunction { get; set; }
        public bool IsSingleExecution { get; set; }
    }
}