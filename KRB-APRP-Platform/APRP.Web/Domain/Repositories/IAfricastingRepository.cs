using APRP.Web.ViewModels.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IAfricastingRepository
    {

        Task<IActionResult> SendSMSToAfricasting(Africasting africasting);

        Task<IActionResult> SendSMSViaMobileSasa(MobileSasa mobileSasa);
    }
}
