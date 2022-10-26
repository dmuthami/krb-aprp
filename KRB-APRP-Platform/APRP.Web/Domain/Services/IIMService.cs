using APRP.Web.ViewModels.Account;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IIMService
    {
        Task<IMResponse> ForgotPassword2(InputModel inputModel);

        Task<IMResponse> ResetPassword2(ResetModel resetModel);

    }
}
