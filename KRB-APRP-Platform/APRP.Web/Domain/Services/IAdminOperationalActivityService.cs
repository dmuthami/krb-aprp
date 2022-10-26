using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IAdminOperationalActivityService
    {
        Task<IEnumerable<AdminOperationalActivity>> ListByAuthorityAsync(long authorityId, long financialYearId);
        Task<AdminOperationalActivityResponse> FindByIdAsync(long ID);
        Task<AdminOperationalActivityResponse> AddAsync(AdminOperationalActivity adminOperationalActivity);
        Task<AdminOperationalActivityResponse> Update(AdminOperationalActivity adminOperationalActivity);
        Task<AdminOperationalActivityResponse> Remove(long ID);
    }
}
