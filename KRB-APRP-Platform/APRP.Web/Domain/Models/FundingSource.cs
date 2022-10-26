using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class FundingSource
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [StringLength(70),Display(Name="Code No.")]
        public string Code { get; set; }

        [Required]
        [StringLength(100), Display(Name = "Source of Funding Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100), Display(Name = "Acronym")]
        public string ShortName { get; set; }

        public ICollection<RevenueCollectionCodeUnit> RevenueCollectionCodeUnits { get; set; }

        public ICollection<FundingSourceSubCode> FundingSourceSubCodes { get; set; }
    }
}