using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class RegionToCounty
    {
        [Key]
        public long ID { get; set; }

        [Display(Name = "Region ID")]
        public long RegionId { get; set; }
        public Region Region { get; set; }

        [Display(Name = "County ID")]
        public long CountyId { get; set; }
        public County County { get; set; }
    }
}
