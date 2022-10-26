using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class AllocationCodeUnit
    {
        public long ID { get; set; }

        [Display(Name = "Allocation Percentage")]
        public double Percent { get; set; }

        [Display(Name = "Agency")]
        public long AuthorityId { get; set; }

        [Display(Name = "Institution")]
        public Authority Authority { get; set; }
    }
}
