using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IDisbursementCodeListService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(DisbursementCodeList disbursementCodeList);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> FindByCodeAsync(string Code);
        Task<GenericResponse> Update(DisbursementCodeList disbursementCodeList);
        Task<GenericResponse> Update(long ID, DisbursementCodeList disbursementCodeList);
        Task<GenericResponse> RemoveAsync(long ID);
        Task<GenericResponse> DetachFirstEntryAsync(DisbursementCodeList disbursementCodeList);
        Task<GenericResponse> FindByDisbursementCodeListEntryAsync(DisbursementCodeList disbursementCodeList);
    }
}
