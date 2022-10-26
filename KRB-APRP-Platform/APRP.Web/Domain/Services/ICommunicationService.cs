using APRP.Web.ViewModels.Account;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ICommunicationService
    {

        Task<CommunicationResponse> SendEmail2(SendEmailModel sendEmailModel);

        Task<CommunicationResponse> SendSMS(ApplicationUser applicationUser, string message);
    }
}
