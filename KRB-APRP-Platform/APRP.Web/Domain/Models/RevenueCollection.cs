using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RevenueCollection
    {
        public long ID { get; set; }

        public double Amount { get; set; }

        [Display(Name = "Revenue Stream")]
        public long RevenueCollectionCodeUnitId { get; set; }
        [Display(Name = "Revenue Stream")]
        public RevenueCollectionCodeUnit RevenueCollectionCodeUnit { get; set; }

    }
}
