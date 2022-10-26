using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IDisbursementTrancheService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(DisbursementTranche disbursementTranche);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> Update(DisbursementTranche disbursementTranche);
        Task<GenericResponse> Update(long ID, DisbursementTranche disbursementTranche);
        Task<GenericResponse> RemoveAsync(long ID);
    }
}
