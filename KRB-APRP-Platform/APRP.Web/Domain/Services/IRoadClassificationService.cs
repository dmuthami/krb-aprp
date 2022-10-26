using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadClassificationService
    {
        Task<RoadClassificationListResponse> ListAsync();

        Task<KenHARoadDictResponse> ListByStatusAsync();
        Task<RoadClassificationListResponse> ListAsync(long AuthorityId);
        Task<RoadClassificationResponse> AddAsync(RoadClassification roadClassification);
        Task<RoadClassificationResponse> FindByIdAsync(long ID);
        Task<RoadClassificationResponse> FindByNameAsync(string RoadName);
        Task<RoadClassificationResponse> FindByRoadIdAsync(string RoadId);
        Task<RoadClassificationResponse> Update(RoadClassification roadClassification);
        Task<RoadClassificationResponse> Update(long ID, RoadClassification roadClassification);
        Task<RoadClassificationResponse> RemoveAsync(long ID);
    }
}
