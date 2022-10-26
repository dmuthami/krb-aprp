using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IShoulderInterventionPavedService
    {
        Task<ShoulderInterventionPavedListResponse> ListAsync();
        Task<ShoulderInterventionPavedResponse> AddAsync(ShoulderInterventionPaved shoulderInterventionPaved);
        Task<ShoulderInterventionPavedResponse> FindByIdAsync(long ID);
        Task<ShoulderInterventionPavedResponse> Update(ShoulderInterventionPaved shoulderInterventionPaved);
        Task<ShoulderInterventionPavedResponse> RemoveAsync(long ID);

    }
}
