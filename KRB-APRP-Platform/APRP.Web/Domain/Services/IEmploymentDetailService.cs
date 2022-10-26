using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IEmploymentDetailService
    {
        Task<IEnumerable<EmploymentDetail>> ListAsync();
        Task<EmploymentDetailResponse> AddAsync(EmploymentDetail employmentDetail);
        Task<EmploymentDetailResponse> FindByIdAsync(long ID);
        Task<EmploymentDetailResponse> UpdateAsync(EmploymentDetail employmentDetail);
        Task<EmploymentDetailResponse> RemoveAsync(long ID);
    }
}
