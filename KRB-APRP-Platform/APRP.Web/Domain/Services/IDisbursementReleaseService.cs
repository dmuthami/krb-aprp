using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IDisbursementReleaseService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(DisbursementRelease disbursementRelease);
        Task<GenericResponse> FindbisbursementReleaseAsync(long ReleaseId, long DisbursementId);
        Task<GenericResponse> Update(DisbursementRelease disbursementRelease);
        Task<GenericResponse> Update(int ID, DisbursementRelease disbursementRelease);
        Task<GenericResponse> RemoveAsync(long ReleaseId, long DisbursementId);
        Task<GenericResponse> Remove(DisbursementRelease disbursementRelease);
    }
}
