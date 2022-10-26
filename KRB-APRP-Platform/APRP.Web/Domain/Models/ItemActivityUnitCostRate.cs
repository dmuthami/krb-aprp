using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ItemActivityUnitCostRate
    {
        public long ID { get; set; }

        [Display(Name = "Item Activity Unit Cost")]
        public long ItemActivityUnitCostId { get; set; }
        public ItemActivityUnitCost ItemActivityUnitCost { get; set; }

        [Display(Name = "Authority Name")]
        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }

        public double AuthorityRate { get; set; }

        [Display(Name = "Financial Year Code")]
        public long FinancialYearId { get; set; }
        public FinancialYear FinancialYear { get; set; }
    }
}
