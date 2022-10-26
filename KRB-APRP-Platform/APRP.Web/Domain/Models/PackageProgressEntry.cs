using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class PackageProgressEntry
    {
        [Key]
        public long ID { get; set; }
        public long PlanActivityId { get; set; }
        public PlanActivity PlanActivity { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public int ReportingMonth { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
