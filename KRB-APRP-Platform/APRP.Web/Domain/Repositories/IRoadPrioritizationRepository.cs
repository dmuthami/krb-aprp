using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadPrioritizationRepository
    {
        Task<IEnumerable<RoadPrioritization>> ListAsync();
        Task AddAsync(RoadPrioritization roadPrioritization);
        Task<RoadPrioritization> FindByIdAsync(long ID);
        Task<RoadPrioritization> FindByNameAsync(string Code);
        void Update(RoadPrioritization roadPrioritization);
        void Update(long ID, RoadPrioritization roadPrioritization);
        void Remove(RoadPrioritization roadPrioritization);
    }
}
