using APRP.Services.AuthorityAPI.Models;
using APRP.Services.AuthorityAPI.Models.Dto;

namespace APRP.Services.AuthorityAPI.Services
{
    public interface IAuthorityService
    {
        Task<GenericResponse> ListAsync(int PageNumber, int PageSize);
        Task<GenericResponse> FindByCodeAsync(string code);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> Update(long ID, Authority authority);
        Task<GenericResponse> RemoveAsync(long ID);
        Task<GenericResponse> AddAsync(Authority authority);
    }
}
