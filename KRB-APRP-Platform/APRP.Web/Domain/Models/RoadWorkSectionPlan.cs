using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadWorkSectionPlan
    {
        [Key]
        public long ID { get; set; }

        public long RoadId { get; set; }
        public Road Road { get; set; }

        public long FinancialYearId { get; set; }
        public FinancialYear FinancialYear { get; set; }

        public long? ConstituencyId { get; set; }
        public virtual Constituency Constituency { get; set; }

        public long RoadSectionId { get; set; }
        public RoadSection RoadSection { get; set; }
        public long? WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }

        public long? FundingSourceId { get; set; }
        public virtual FundingSource FundingSource { get; set; }

        public long? FundTypeId { get; set; }
        public virtual FundType FundType { get; set; }

        public long? ExecutionMethodId { get; set; }
        public virtual ExecutionMethod ExecutionMethod { get; set; }

        public double TotalEstimateCost { get; set; }
        [Display(Name ="Planned Length")]
        public double PlannedLength { get; set; }
        [Display(Name = "Achieved KM Length")]
        public double AchievedLength { get; set; }

        public bool  RevisionStatus { get; set; }
        public DateTime RevisionDate { get; set; }
        public string RevisionBy { get; set; }
        public DateTime  CreatedDate{ get; set; }
        public string CreatedBy{ get; set; }
        public DateTime UpdateDate{ get; set; }
        public string UpdateBy{ get; set; }

        public bool  ApprovalStatus { get; set; }

        public bool ApprovalStatusInternal { get; set; }

        public long? WorkplanApprovalBatchId { get; set; }
        public WorkplanApprovalBatch WorkplanApprovalBatch { get; set; }

        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }

        public List<PlanActivity> PlanActivities { get; set; }
        public List<PlanActivityPBC> PlanActivityPBCs { get; set; }

    }
}
