using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class FundType
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(15)]
        public string Code { get; set; }

        [Required]
        [StringLength(70)]
        public string Name { get; set; }
    }
}