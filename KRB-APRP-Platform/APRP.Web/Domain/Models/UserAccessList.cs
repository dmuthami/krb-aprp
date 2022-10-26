using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public enum InstitutionGroup
    {
        KeNHA = 1,
        KeRRA = 2,
        KURA = 3,
        KWS = 4,
        KRB = 8,
        CGs = 55,
        RSIP_CS = 0
    }
    public class UserAccessList
    {
        public long Id { get; set; }

        [Display(Name = "Middle Name"), MaxLength(25)]
        public string? MiddleName { get; set; }

        [Display(Name = "First Name"), MaxLength(25)]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name"), MaxLength(25)]
        public string? LastName { get; set; }

        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Display(Name = "National ID"), MaxLength(25)]
        public string? NationalID { get; set; }

        [Display(Name = "Mobile No"), MaxLength(25)]
        public string? MobileNo { get; set; }

        [Display(Name = "Designation")]
        public string? Designation { get; set; }

        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Display(Name = "Institution Group")]
        public InstitutionGroup InstitutionGroup { get; set; }

        [Display(Name = "Institution")]
        public long AuthorityId { get; set; }
        [Display(Name = "Institution")]
        public Authority Authority { get; set; }
    }
}
