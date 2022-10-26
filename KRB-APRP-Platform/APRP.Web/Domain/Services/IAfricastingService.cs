using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.DTO;

namespace APRP.Web.Domain.Services
{
    public interface IAfricastingService
    {
        Task<GenericResponse> SendSMSToAfricasting(Africasting africasting);
        Task<GenericResponse> SendSMSViaMobileSasa(MobileSasa mobileSasa);

    }
}
