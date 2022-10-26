using System.ComponentModel.DataAnnotations;

namespace APRP.Web.ViewModels.UserViewModels
{
    public class UpdateProfileDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please enter your Email address as requested.")]
        [EmailAddress(ErrorMessage = "The Email address is not valid")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Surburb"), MaxLength(25)]
        public string Surburb { get; set; }

        [Display(Name = "House Number"), MaxLength(25)]
        public string HouseNumber { get; set; }

        [Display(Name = "Street Name"), MaxLength(25)]
        public string Street_Name { get; set; }

        [Display(Name = "Postal Address"), MaxLength(25)]
        public string Postal_Address { get; set; }

        [Display(Name = "Postal Code"), MaxLength(25)]
        public string Postal_Code { get; set; }

        [Display(Name = "Surname"), MaxLength(25)]
        public string Surname { get; set; }

        [Display(Name = "Full Name")]
        public string Full_Name { get; set; }

        [Required(ErrorMessage = "Please enter a valid KRA PIN.")]
        [Display(Name = "KRA PIN")]
        public string KRA_Pin { get; set; }

        [Display(Name = "Primary Phone Number")]
        public string PrimaryPhoneNumber { get; set; }

        [Display(Name = "Secondary Phone Number")]
        public string SecondaryPhoneNumber { get; set; }
    }
}
