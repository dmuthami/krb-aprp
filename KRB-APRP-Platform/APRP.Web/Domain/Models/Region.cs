using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Region
    {
        [Key]
        public long ID { get; set; }

        [StringLength(70)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Authority ID")]
        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }

        [Display(Name = "Relationship Manager")]
        public string RelationshipManager { get; set; }

        [Display(Name = "RM Mobile No")]
        public string RMMobileNo { get; set; }

        [Display(Name = "Office Location")]
        public string OfficeLocation { get; set; }

        [Display(Name = "RM Office Email")]
        public string RMOfficeEmail { get; set; }

        [Display(Name = "RM Personal Email")]
        public string RMPersonalEmail { get; set; }

        [Display(Name = "Postal Address")]
        public string PostalAddress { get; set; }

        [Display(Name = "Postal Address")]
        public string PhysicalAddress { get; set; }

        [Display(Name = "Office Tel.")]
        public string OfficeTel { get; set; }

        [Display(Name = "Fax")]
        public string Fax { get; set; }

        public ICollection<RegionToCounty> RegionToCountys { get; set; }

        public ICollection<KWSPark> KWSParks { get; set; }
    }
}
