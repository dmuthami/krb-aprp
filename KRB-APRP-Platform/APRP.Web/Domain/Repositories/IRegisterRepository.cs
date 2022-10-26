using APRP.Web.Domain.Models;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IRegisterRepository
    {
        Task<IActionResult> ListAsync2();

        Task<IList<UserListViewModel>> ListAsync();
        Task<AuthResponse> AddAsync(Register register);
        Task<IActionResult> UpdateAsync(ApplicationUser applicationUser);
        Task<AuthResponse> Remove(string ID);
    }
}
