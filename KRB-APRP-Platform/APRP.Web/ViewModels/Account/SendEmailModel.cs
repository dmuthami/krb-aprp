using System.ComponentModel.DataAnnotations;

namespace APRP.Web.ViewModels.Account
{
    public class SendEmailModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Subject { get; set; }

        public string HTMLMessage { get; set; }
    }
}
