using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Allocation
    {
        public long ID { get; set; }
        public double Amount { get; set; }

        [Display(Name = "Allocation Code")]
        public long AllocationCodeUnitId { get; set; }
        [Display(Name = "Allocation Code")]
        public AllocationCodeUnit AllocationCodeUnit { get; set; }

        [Display(Name = "Financial Year")]
        public long FinancialYearId { get; set; }

        [Display(Name = "Financial Year")]
        public FinancialYear FinancialYear { get; set; }
    }
}



