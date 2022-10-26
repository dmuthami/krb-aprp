using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ICountiesRoadRepository
    {
        Task<CountiesRoad> FindByIdAsync(long ID);

        Task<CountiesRoad> FindByRoadNumberAsync(string RoadNumber);

        Task<IEnumerable<CountiesRoad>> ListAsync();

        Task<IEnumerable<CountiesRoad>> ListAsync(string RoadNumber);

        Task<IQueryable<CountiesRoad>> ListViewAsync(long AuthorityId);

        void Update(long ID, CountiesRoad countiesRoad);

        Task AddAsync(CountiesRoad countiesRoad);

        void Remove(CountiesRoad countiesRoad);
    }
}
