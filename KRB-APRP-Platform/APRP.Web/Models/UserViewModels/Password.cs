using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Models.UserViewModels
{
    public class ChangePassword
    {
        [Required]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string old_password { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string new_password { get; set; }

    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Please enter valid Email.")]
        [Display(Name = "Email"), MaxLength(80)]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }


        public string? url { get; set; }
    }

    public class ResetPasswordConfirm
    {
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string uid { get; set; }

        public string token { get; set; }
    }
}
