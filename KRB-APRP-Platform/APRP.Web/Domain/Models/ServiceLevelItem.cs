using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ServiceLevelItem
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        public string Description { get; set; }

        public long ServiceLevelGroupId { get; set; }
        public ServiceLevelGroup ServiceLevelGroup { get; set; }

    }
}
