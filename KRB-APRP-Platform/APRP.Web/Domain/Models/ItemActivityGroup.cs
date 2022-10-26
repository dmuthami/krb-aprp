using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ItemActivityGroup
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(4)]
        public string BillNumber { get; set; }

        [Required]
        public string Description { get; set; }

        public IEnumerable<ItemActivityUnitCost> ItemActivityUnitCosts { get; set; }


    }
}
