using System.Collections.Generic;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class UsedImpuritiesItemInSequenceItem
    {
        public long Id { get; set; }
        //public ResultIoOutputItem ResultIoOutputItem { get; set; }
        public List<ResultIoOutputItem> ResultIoOutputItems { get; set; }
        public long IdImpurityItem { get; set; }
        public SpecificationItem SpecificationItem { get; set; }
        public UsedImpuritiesItemInSequenceItem() {
            SpecificationItem = new SpecificationItem();
            //ResultIoOutputItem = new ResultIoOutputItem() {  HighRangeValue = 1};
            ResultIoOutputItems = new List<ResultIoOutputItem>();

        }

        public UsedImpuritiesItemInSequenceItem CopyItem() {
            UsedImpuritiesItemInSequenceItem copy = new UsedImpuritiesItemInSequenceItem() {
                Id = this.Id,
                IdImpurityItem = this.IdImpurityItem
            };
            foreach(ResultIoOutputItem resultIoOutputItems in ResultIoOutputItems)
            {
                copy.ResultIoOutputItems.Add(resultIoOutputItems);
            }
            //copy.ResultIoOutputItem = this.ResultIoOutputItem.CopyItem();
            copy.SpecificationItem = this.SpecificationItem.CopyItem();
            return copy;
        }
    }
}
