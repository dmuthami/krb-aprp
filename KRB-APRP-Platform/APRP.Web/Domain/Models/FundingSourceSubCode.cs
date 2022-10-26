using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class FundingSourceSubCode
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(70), Display(Name = "Sub Code")]
        public string Code { get; set; }

        [Required]
        [StringLength(100), Display(Name = "Acronym")]
        public string ShortName { get; set; }

        [Required]
        [StringLength(100), Display(Name = "Sub Code Full Name")]
        public string Name { get; set; }

        [Display(Name = "Funding Source ID")]
        public long FundingSourceId { get; set; }
        public FundingSource FundingSource { get; set; }
    }
}
