using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ServiceLevelGroup
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        public string Description { get; set; }

        public IEnumerable<ServiceLevelItem> ServiceLevelItems { get; set; }
    }
}
