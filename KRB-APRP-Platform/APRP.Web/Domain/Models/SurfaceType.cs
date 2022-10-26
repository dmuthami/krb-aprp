using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class SurfaceType
    {
        public long ID { get; set; }

        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        [StringLength(75)]
        public string Description { get; set; }

    }
}
