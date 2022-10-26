using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ExecutionMethod
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(35)]
        public string Code { get; set; }
        [StringLength(70)]
        public string Name { get; set; }

    }
}
