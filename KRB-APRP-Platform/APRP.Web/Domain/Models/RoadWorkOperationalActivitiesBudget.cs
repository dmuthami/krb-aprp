using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadWorkOperationalActivitiesBudget
    {
        [Key]
        public long ID { get; set; }
        public long RoadWorkBudgetHeaderId { get; set; }
        public RoadWorkBudgetHeader RoadWorkBudgetHeader { get; set; }

        [Required]
        public long FundingSourceId { get; set; }
        public FundingSource FundingSource { get; set; }

        [Required]
        public long FundTypeId { get; set; }
        public FundType FundType { get; set; }
        public double OverHeadBudget { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
