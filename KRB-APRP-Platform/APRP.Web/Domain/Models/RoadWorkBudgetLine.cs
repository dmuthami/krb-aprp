using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RoadWorkBudgetLine
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public long FundingSourceId { get; set; }
        public FundingSource FundingSource { get; set; } 

        [Required]
        public long FundTypeId { get; set; }
        public FundType FundType { get; set; }

        public double Administration { get; set; }
        public double RoutineMaintanance { get; set; }

        public double PeriodicMentanance { get; set; }

        public double SpotImprovement { get; set; }

        public double StructureConstruction { get; set; }

        public double PBCManagement { get; set; }

        public double RAM { get; set; }

        public double Network { get; set; }

        public double WeighBridges { get; set; }

        public double AlehuOperationsAndPurchaseOfVehicles { get; set; }

        public double AxleLoadActivities { get; set; }

        public double EmergencyWorks { get; set; }

        public double RoadSafety { get; set; }

        public double RoadConditionSurvey { get; set; }

        public double BasedContracts { get; set; }

        public double Matters { get; set; }

        public double Support { get; set; }

        public double HQActivities { get; set; }

        public double RSIPCriticalLinks { get; set; }

        public double RehabilitationWork { get; set; }
        public double NewStructure { get; set; }

        public double Total { get; set; }

        public long RoadWorkBudgetHeaderId { get; set; }
        public  RoadWorkBudgetHeader RoadWorkBudgetHeader { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
