using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services
{
    public interface IRegisterService
    {
        Task<RegisterListResponse> ListAsync();

        Task<RegisterResponse> ListAsync2();
        Task<RegisterResponse> AddAsync(Register register);

        Task<RegisterResponse> AddAsync2(Register register);

        Task<RegisterResponse> UpdateAsync(ApplicationUser applicationUser);
        Task<RegisterResponse> RemoveAsync(string ID);

    }
}
