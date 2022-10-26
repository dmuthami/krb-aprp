using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadSectionRepository
    {
        Task<IEnumerable<RoadSection>> ListAsync();

        Task<IQueryable<RoadSection>> ListViewAsync();

        Task<IQueryable<RoadSection>> ListViewAsync(Authority authority);

        Task<IEnumerable<RoadSection>> ListByRoadIdAsync(long roadID);

        Task<IEnumerable<RoadSection>> ListUnPlannedSectionsByRoadIdAsync(long roadId, long financialYearId);
        Task<IEnumerable<RoadSection>> GetRoadSectionsForAgencyAsync(Authority authority, string SurfaceType);

        Task AddAsync(RoadSection roadSection);
        Task<RoadSection> FindByIdAsync(long ID);
        Task<RoadSection> FindBySectionIdAsync(string SectionID);

        Task<RoadSection> FindBySectionIdAsync(string SectionID, long AuthorityId);
        void Update(RoadSection roadSection);

        void Update(long ID, RoadSection roadSection);

        void Remove(RoadSection roadSection);

        Task DetachFirstEntryAsync(RoadSection roadSection);

        Task<RoadSection> FindByDisbursementEntryAsync(RoadSection roadSection);
    }
}
