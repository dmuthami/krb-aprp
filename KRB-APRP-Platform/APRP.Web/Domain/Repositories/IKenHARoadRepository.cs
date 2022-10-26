using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IKenHARoadRepository
    {
        Task<KenhaRoad> FindByIdAsync(long ID);

        Task<KenhaRoad> FindByRoadNumberAsync(string RoadNumber);

        Task<KenhaRoad> FindBySectionIdAsync(string SectionID);

        Task<IEnumerable<KenhaRoad>> ListAsync();

        Task<IEnumerable<KenhaRoad>> ListAsync(string RoadNumber);

        Task<IQueryable<KenhaRoad>> ListViewAsync();

        void Update(long ID, KenhaRoad kenhaRoad);

        Task AddAsync(KenhaRoad kenhaRoad);

        void Remove(KenhaRoad kenhaRoad);
    }
}
