using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Models.UserViewModels
{
    public class ApplicationUser
    {
        [Required(ErrorMessage = "The username is required"),
         MaxLength(15, ErrorMessage = "A username cannot exceed 40 characters")]
        [Display(Name = "Username")]
        public string username { get; set; }

        [Display(Name = "Email")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string email { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(255, ErrorMessage = "First Name cannot exceed 255 characters")]
        public string first_name { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(255, ErrorMessage = "Last Name cannot exceed 255 characters")]
        public string? last_name { get; set; }

        public bool is_active { get; set; } = true;

        public bool is_staff { get; set; } = false;

        [MaxLength(17, ErrorMessage = "Phone number cannot exceed 17 characters")]
        public string mobile_phone { get; set; }

        [Display(Name = "Date of Birth")]
        public DateOnly? dob { get; set; }

    }
}
