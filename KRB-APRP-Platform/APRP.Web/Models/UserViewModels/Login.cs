using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Models.UserViewModels
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "Send OTP to Email")]
        public bool sendOTPtoEmail { get; set; }

        public string? ReturnUrl { get; set; }

        public string? message { get; set; }

    }

    #region Sign In Verification

    public class RegistrationDTO
    {
        public string? phone { get; set; }
        public string? otp { get; set; }
        public long userid { get; set; }
    }
    #endregion
}
