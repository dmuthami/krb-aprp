using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Models.UserViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Please enter valid username.")]
        [Display(Name = "Username"), MaxLength(40)]
        public string username { get; set; }

        [Required(ErrorMessage = "Please enter valid Email.")]
        [Display(Name = "Email"), MaxLength(80)]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name"), MaxLength(255)]
        public string first_name { get; set; }

        [Display(Name = "Last Name"), MaxLength(255)]
        public string? last_name { get; set; }

        [Required(ErrorMessage = "Please enter valid Mobile No.")]
        [Display(Name = "Mobile No."), MaxLength(17)]
        public string mobile_phone { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ValidatePhoneSendOTP
    {

        [Required(ErrorMessage = "Please enter valid Email.")]
        [Display(Name = "Email"), MaxLength(80)]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required(ErrorMessage = "Please enter valid Mobile No.")]
        [Display(Name = "Mobile No."), MaxLength(17)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string phone { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Display(Name = "Send OTP to Email")]
        public bool sendOTPtoEmail { get; set; }=false;
    }

    public class ValidateOTP
    {

        [Required(ErrorMessage = "Please enter a valid Email.")]
        [Display(Name = "Email"), MaxLength(80)]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required(ErrorMessage = "Please enter valid Mobile No.")]
        [Display(Name = "Mobile No."), MaxLength(17)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string phone { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Required(ErrorMessage = "OTP is Required")]
        public string? otp { get; set; }
    }

}
