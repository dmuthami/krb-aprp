using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public enum RevenueStream
    {
        Fuel_Levy=0,
        Transit_Tolls=1
    }
    public class RevenueCollectionCodeUnit
    {
        public long ID { get; set; }

        [Display(Name = "Revenue Stream")]
        public RevenueStream RevenueStream { get; set; }

        [Display(Name = "Financial Year")]
        public long FinancialYearId { get; set; }

        [Display(Name = "Financial Year")]
        public FinancialYear FinancialYear { get; set; }

        [Display(Name = "Revenue Collection Code Unit Type")]
        public long RevenueCollectionCodeUnitTypeId { get; set; }

        [Display(Name = "Revenue Collection Code Unit Type")]
        public RevenueCollectionCodeUnitType RevenueCollectionCodeUnitType { get; set; }

        [Display(Name = "Authority")]
        public long AuthorityId { get; set; }

        [Display(Name = "Authority")]
        public Authority Authority { get; set; }
        public RevenueCollection RevenueCollection { get; set; }

        [Display(Name = "Funding Source")]
        public long FundingSourceId { get; set; }

        [Display(Name = "Funding Source")]
        public FundingSource FundingSource { get; set; }

    }
}
