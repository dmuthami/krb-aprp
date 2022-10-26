using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IKeRRARoadRepository
    {
        Task<KerraRoad> FindByIdAsync(long ID);

        Task<KerraRoad> FindByRoadNumberAsync(string RoadNumber);

        Task<KerraRoad> FindBySectionIdAsync(string SectionID);

        Task<IEnumerable<KerraRoad>> ListAsync();

        Task<IEnumerable<KerraRoad>> ListAsync(string RoadNumber);

        Task<IQueryable<KerraRoad>> ListViewAsync();

        void Update(long ID, KerraRoad kerraRoad);

        Task AddAsync(KerraRoad kerraRoad);

        void Remove(KerraRoad kerraRoad);
    }
}
