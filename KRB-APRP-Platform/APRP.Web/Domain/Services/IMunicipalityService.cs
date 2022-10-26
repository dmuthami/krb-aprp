using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IMunicipalityService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(Municipality municipality);
        Task<GenericResponse> FindByIdAsync(int ID);
        Task<GenericResponse> Update(Municipality municipality);
        Task<GenericResponse> Update(int ID, Municipality municipality);
        Task<GenericResponse> RemoveAsync(int ID);
    }
}
