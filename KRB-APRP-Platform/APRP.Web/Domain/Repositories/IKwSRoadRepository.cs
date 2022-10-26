using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IKwSRoadRepository
    {
        Task<KwsRoad> FindByIdAsync(long ID);

        Task<KwsRoad> FindByRoadNumberAsync(string RoadNumber);

        Task<KwsRoad> FindBySectionIdAsync(string SectionID);

        Task<IEnumerable<KwsRoad>> ListAsync();

        Task<IEnumerable<KwsRoad>> ListAsync(string RoadNumber);

        Task<IQueryable<KwsRoad>> ListViewAsync();

        void Update(long ID, KwsRoad kwsRoad);

        Task AddAsync(KwsRoad kwsRoad);

        void Remove(KwsRoad kwsRoad);
    }
}
