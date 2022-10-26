using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkPlanPackage
    {
        [Key]
        public long ID { get; set; }


        [Display(Name ="Package Number")]
        [Required]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Package Name")]
        public string Name { get; set; }
        public IEnumerable<WorkpackageRoadWorkSectionPlan> WorkpackageRoadWorkSectionPlans { get; }
        public long FinancialYearId { get; set; }
        public FinancialYear FinancialYear { get; set; }
        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }

        [Display(Name = "Engineer Estimate")]
        public double EngineerEstimate { get; set; }
        [Display(Name="Original Contract Sum")]
        public double OriginalContractSum { get; set; }

        [Display(Name = "% Contigencies")]
        public float Contigencies { get; set; }

        [Display(Name = "Variation Amount")]
        public double VariationAmount { get; set; }
        [Display(Name = "Varation Percentage")]
        public float VariationPercentage { get; set; }

        [Display(Name = "Package Status")]
        public int Status { get; set; }

        public float VAT { get; set; }

        [Display(Name="Total Achieved Km Up To Date")]
        public double AchievedKilometersToDate { get; set; }
    }
}
