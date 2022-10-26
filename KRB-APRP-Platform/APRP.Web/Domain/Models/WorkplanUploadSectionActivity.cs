using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkplanUploadSectionActivity
    {
        [Key]
        public long ID { get; set; }
        public string ActivityCode { get; set; }
        public string AcivityDescription { get; set; }
        public string Technology { get; set; }
        public string UnitMeasure { get; set; }
        public decimal Quantity { get; set; }
        public double PlannedRate { get; set; }
        public double PlannedAmount{ get; set; }
        public long WorkplanUploadSectionId { get; set; }
        public WorkplanUploadSection WorkplanUploadSection { get; set; }

    }
}
