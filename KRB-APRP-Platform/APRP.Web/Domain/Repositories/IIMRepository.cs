using APRP.Web.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IIMRepository
    {
        Task<IActionResult> ForgotPassword2(InputModel inputModel);
    }
}
