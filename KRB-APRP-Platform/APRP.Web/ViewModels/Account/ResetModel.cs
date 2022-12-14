using System.ComponentModel.DataAnnotations;

namespace APRP.Web.ViewModels.Account
{
    public class ResetModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage ="The password entered do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public string Id { get; set; }
    }
}
