using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class QuarterCodeUnit
    {
        public long ID { get; set; }

        [Display(Name = "Quarter Code Item")]
        public long QuarterCodeListId { get; set; }

        [Display(Name = "Quarter Code Item")]
        public QuarterCodeList QuarterCodeList { get; set; }

        [Display(Name = "Financial Year")]
        public long FinancialYearId { get; set; }

        [Display(Name = "Financial Year")]
        public FinancialYear FinancialYear { get; set; }
    }
}
