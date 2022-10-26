using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services
{
    public interface IAuthenticateService
    {
        Task<AuthenticateResponse> LoginAsync2(LoginModel loginModel);

        Task<AuthenticateResponse> CheckToken(CheckToken checkToken);
    }
}
