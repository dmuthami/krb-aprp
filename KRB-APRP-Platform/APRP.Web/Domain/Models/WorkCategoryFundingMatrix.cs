using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkCategoryFundingMatrix
    {
        public long ID { get; set; }

        [Display(Name = "Administration")]
        public double Administration { get; set; }

        [Display(Name = "Routine Maintenance")]
        public double RoutineMaintenance { get; set; }

        [Display(Name = "Periodic Maintenance")]
        public double PeriodicMentanance { get; set; }

        [Display(Name = "Spot Improvement")]
        public double SpotImprovement { get; set; }

        [Display(Name = "Structure Construction")]
        public double StructureConstruction { get; set; }

        [Display(Name = "PBC Management")]
        public double PBCManagement { get; set; }

        [Display(Name = "RAM")]
        public double RAM { get; set; }

        [Display(Name = "Network")]
        public double Network { get; set; }

        [Display(Name = "WEIGH BRIDGES")]
        public double WeighBridges { get; set; }

        [Display(Name = "ALEHU OPERATIONS  AND PURCHASE OF VEHICLES")]
        public double AlehuOperationsAndPurchaseOfVehicles { get; set; }

        [Display(Name = "AXLE LOAD ACTIVITIES")]
        public double AxleLoadActivities { get; set; }

        [Display(Name = "EMERGENCY WORKS")]
        public double EmergencyWorks { get; set; }

        [Display(Name = "Road Safety")]
        public double RoadSafety { get; set; }

        [Display(Name = "Road Condition Survey")]
        public double RoadConditionSurvey { get; set; }

        [Display(Name = "Based Contracts")]
        public double BasedContracts { get; set; }

        [Display(Name = "Matters")]
        public double Matters { get; set; }

        [Display(Name = "Support")]
        public double Support { get; set; }

        [Display(Name = "HQ Activities")]
        public double HQActivities { get; set; }

        [Display(Name = "RSIP Critical Links")]
        public double RSIPCriticalLinks { get; set; }

        [Display(Name = "Rehabilitation Work")]
        public double RehabilitationWork { get; set; }

        [Display(Name = "New Structure")]
        public double NewStructure { get; set; }

        [Display(Name = "Authority")]
        public long AuthorityId { get; set; }

        [Display(Name = "Authority")]
        public Authority Authority { get; set; }

        [Display(Name = "Financial Year")]
        public long FinancialYearId { get; set; }

        [Display(Name = "Financial Year")]
        public FinancialYear FinancialYear { get; set; }
    }
}
