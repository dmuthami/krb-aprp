using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ItemActivityPBC
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        public long TechnologyId { get; set; }
        public Technology Technology { get; set; }

    }
}
