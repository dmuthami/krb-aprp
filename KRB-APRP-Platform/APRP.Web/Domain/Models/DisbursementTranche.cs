using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class DisbursementTranche
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(70), Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [StringLength(100), Display(Name = "Tranche Name")]
        public string Name { get; set; }
        public ICollection<Disbursement> Disbursement { get; set; }
    }
}
