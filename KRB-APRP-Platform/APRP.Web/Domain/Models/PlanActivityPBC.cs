using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class PlanActivityPBC
    {
        [Key]
        public long ID { get; set; }
        public long RoadWorkSectionPlanId { get; set; }
        public RoadWorkSectionPlan RoadWorkSectionPlan { get; set; }
        public long ItemActivityPBCId { get; set; }
        public ItemActivityPBC ItemActivityPBC{ get; set; }
        [Display(Name = "KM Planned.")]
        public double PlannedKM { get; set; }
        [Display(Name = "KM Achieved.")]
        public double AchievedKM { get; set; }
        [Display(Name = "Cost Per KM per Month.")]
        public double CostPerKMPerMonth { get; set; }
        [Display(Name = "Total Amount")]
        public double TotalAmount { get; set; }
       
    }
}
