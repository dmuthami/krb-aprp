using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IGISRoadRepository
    {
        Task<IEnumerable<GISRoad>> ListAsync();
        Task AddAsync(GISRoad gISRoad);
        Task<GISRoad> FindByIdAsync(long ID);
        void Update(GISRoad gISRoad);
        void Remove(GISRoad gISRoad);
        Task<IEnumerable<GISRoad>> ListByRoadNumberAsync(string RoadNumber);
        Task<GISRoad> FindByRoadNumberAsync(string RoadNumber);
    }
}
