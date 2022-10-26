using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadPrioritizationService
    {
        Task<RoadPrioritizationListResponse> ListAsync();
        Task<RoadPrioritizationResponse> AddAsync(RoadPrioritization roadPrioritization);
        Task<RoadPrioritizationResponse> FindByIdAsync(long ID);

        Task<RoadPrioritizationResponse> FindByNameAsync(string Code);
        Task<RoadPrioritizationResponse> Update(RoadPrioritization roadPrioritization);
        Task<RoadPrioritizationResponse> Update(long ID, RoadPrioritization roadPrioritization);
        Task<RoadPrioritizationResponse> RemoveAsync(long ID);
    }
}
