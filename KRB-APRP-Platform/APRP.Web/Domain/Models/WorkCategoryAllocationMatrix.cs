using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkCategoryAllocationMatrix
    {
        public int ID { get; set; }

        [Display(Name = "Authority")]
        public long AuthorityId { get; set; }

        [Display(Name = "Authority")]
        public Authority Authority { get; set; }

        [Display(Name = "Financial Year")]
        public long FinancialYearId { get; set; }

        [Display(Name = "Financial Year")]
        public FinancialYear FinancialYear { get; set; }

        public double Percent { get; set; }

        public long WorkCategoryId { get; set; }
        public WorkCategory WorkCategory { get; set; }
    }
}
