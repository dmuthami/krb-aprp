using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IARICSYearService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(ARICSYear aRICSYear);
        Task<GenericResponse> FindByIdAsync(int ID);
        Task<GenericResponse> FindByYearAsync(int ARICSYear);
        Task<GenericResponse> Update(ARICSYear aRICSYear);
        Task<GenericResponse> Update(int ID, ARICSYear aRICSYear);
        Task<GenericResponse> RemoveAsync(int ID);
        Task<GenericResponse> DetachFirstEntryAsync(ARICSYear aRICSYear);
    }
}
