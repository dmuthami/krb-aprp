using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    // Add profile data for application users 
    //by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //[Required]
        //[Column("surname")]
        [Display(Name = "Surname"), MaxLength(25)]
        public string? Surname { get; set; }

        //[Required]
        //[Column("other_names")]
        [Display(Name = "Other Names"), MaxLength(25)]
        public string? OtherNames { get; set; }

        //[Column("updated_bio")]
        [Display(Name = "Updated Bio"), MaxLength(25)]
        public bool UpdatedBio { get; set; }

        //[Column("secondary_number")]
        [Display(Name = "Secondary Number")]
        public string? SecondaryNumber { get; set; }

        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }
    }
}
