using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IKWSParkService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(KWSPark kWSPark);
        Task<GenericResponse> FindByIdAsync(int ID);
        Task<GenericResponse> Update(KWSPark kWSPark);
        Task<GenericResponse> Update(int ID, KWSPark kWSPark);
        Task<GenericResponse> RemoveAsync(int ID);
    }
}
