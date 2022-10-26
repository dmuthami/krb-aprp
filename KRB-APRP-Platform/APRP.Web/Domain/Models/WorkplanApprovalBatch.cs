using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkplanApprovalBatch
    {
        [Key]
        public long ID { get; set; }
        public int ApprovalStatus { get; set; }
        public bool isAricsDone { get; set; }
        public bool isRoadSafetyEnvironmentDone { get; set; }
        public bool isRoadRelateCourseDone { get; set; }
        public bool isUnitRatesEstimateDone { get; set; }
        public bool isR2000AspectsDone { get; set; }
        public virtual IEnumerable<RoadWorkSectionPlan> RoadWorkSectionPlans { get; set; }
        public virtual IEnumerable<WorkplanApprovalHistory> WorkplanApprovalHistories { get; set; }
        public long FinancialYearId { get; set; }
        public FinancialYear FinancialYear { get; set; }
        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }
    }
}
