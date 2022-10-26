using APRP.Web.Domain.Models;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.ViewModels.ResponseTypes
{
    public class AuthResponse : ActionResult
    {
        public string StatusCode { get; set; }

        public string Response { get; set; }

        public string InnerException { get; set; }

        public string StackTrace { get; set; }

        public IdentityResult result { get; set; }

        public bool Success { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public LoginToken LoginToken { get; set; }
    }
}
