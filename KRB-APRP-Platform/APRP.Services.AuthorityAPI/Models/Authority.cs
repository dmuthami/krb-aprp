using System.ComponentModel.DataAnnotations;

namespace APRP.Services.AuthorityAPI.Models
{
    public class Authority
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        [Required]
        [StringLength(70)]
        public string Name { get; set; }

        public int? Type { get; set; }

        public string? Number { get; set; }
    }
}
