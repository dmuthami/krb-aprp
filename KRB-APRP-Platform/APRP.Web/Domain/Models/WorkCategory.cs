using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkCategory
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
