using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Technology
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        [Required]
        [StringLength(70)]
        public string Description { get; set; }

    }
}