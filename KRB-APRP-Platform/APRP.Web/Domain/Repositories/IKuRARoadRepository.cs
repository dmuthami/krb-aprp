using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Repositories
{
    public interface IKuRARoadRepository
    {
        Task<IEnumerable<KuraRoad>> ListAsync();

        Task<IEnumerable<KuraRoad>> ListAsync(string RoadNumber);

        Task<KuraRoad> FindByIdAsync(long ID);

        Task<KuraRoad> FindByRoadNumberAsync(string RoadNumber);

        Task<IQueryable<KuraRoad>> ListViewAsync();

        void Update(long ID, KuraRoad kuraRoad);

        Task AddAsync(KuraRoad kuraRoad);

        void Remove(KuraRoad kuraRoad);
    }
}
