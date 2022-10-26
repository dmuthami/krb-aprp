using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class OtherFundItem
    {
        public long ID { get; set; }

        public string Description { get; set; }
        public double Amount { get; set; }

        [Display(Name = "Financial Year")]
        public long FinancialYearId { get; set; }
        public FinancialYear FinancialYear { get; set; }
    }
}
