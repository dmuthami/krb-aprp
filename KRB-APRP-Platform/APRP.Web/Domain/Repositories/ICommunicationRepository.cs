using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface ICommunicationRepository
    {
        Task<IActionResult> SendSMS(ApplicationUser applicationUser, string message);

    }
}
