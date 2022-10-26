using System.ComponentModel.DataAnnotations;

namespace APRP.Web.ViewModels.Account
{
    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Code { get; set; }
        public string Id { get; set; }
        public string StatusCode { get; set; }
    }
}
