using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkpackageRoadWorkSectionPlan
    {
        [Key]
        public long ID { get; set; }
        public long WorkPlanPackageId { get; set; }
        public WorkPlanPackage WorkPlanPackage { get; set; }
        public long RoadWorkSectionPlanId { get; set; }
        public RoadWorkSectionPlan RoadWorkSectionPlan { get; set; }

        public double PackageQuantity { get; set; }
        public float EngineerRate { get; set; }
        public double PackageAmount { get; set; }

        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }


    }
}
