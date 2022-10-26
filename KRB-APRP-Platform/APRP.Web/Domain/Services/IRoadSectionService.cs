using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadSectionService
    {
        Task<RoadSectionListResponse> ListAsync();
        Task<RoadSectionViewModelResponse> ListViewAsync();
        Task<RoadSectionViewModelResponse> ListViewAsync(Authority authority);
        Task<RoadSectionListResponse> ListByRoadIdAsync(long roadID);
        Task<IEnumerable<RoadSection>> ListUnPlannedSectionsByRoadIdAsync(long roadId, long financialYearId);
        Task<RoadSectionListResponse> GetRoadSectionsForAgencyAsync(Authority authority, string SurfaceType);
        Task<RoadSectionResponse> AddAsync(RoadSection roadSection);
        Task<RoadSectionResponse> FindByIdAsync(long ID);
        Task<RoadSectionResponse> FindBySectionIdAsync(string SectionID);
        Task<RoadSectionResponse> FindBySectionIdAsync(string SectionID, long AuthorityId);
        Task<RoadSectionResponse> UpdateAsync(RoadSection roadSection);
        Task<RoadSectionResponse> UpdateAsync(long ID, RoadSection roadSection);
        Task<RoadSectionResponse> RemoveAsync(long ID);
        Task<RoadSectionResponse> FindByDisbursementEntryAsync(RoadSection roadSection);
        Task<RoadSectionResponse> DetachFirstEntryAsync(RoadSection roadSection);
    }
}
