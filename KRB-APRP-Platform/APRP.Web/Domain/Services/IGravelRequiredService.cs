using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IGravelRequiredService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(GravelRequired gravelRequired);
        Task<GenericResponse> FindByIdAsync(int ID);
        Task<GenericResponse> Update(GravelRequired gravelRequired);
        Task<GenericResponse> Update(int ID, GravelRequired gravelRequired);
        Task<GenericResponse> RemoveAsync(int ID);
    }
}
