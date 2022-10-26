using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadRepository
    {
        Task<IEnumerable<Road>> ListAsync();

        Task<IQueryable<Road>> ListViewAsync();

        Task<IQueryable<Road>> ListViewAsync(Authority authority);

        Task<IQueryable<Road>> ListViewWithAricsAsync(Authority authority, int? Year);

        Task<IQueryable<Road>> ListViewWithAricsAsync(int? Year);

        Task<IActionResult> ListAsync(Authority authority);

        Task<IEnumerable<Road>> GetRoadWithSectionsAsync(Authority authority);

        Task AddAsync(Road road);
        Task<Road> FindByDisbursementEntryAsync(Road road);
        Task<Road> FindByIdAsync(long ID);

        Task<Road> FindByIdAsync(long ID, int ARICSYear);

        Task<Road> FindByRoadNumberAsync(string RoadNumber);

        Task<Road> FindByRoadNumberAsync(long AuthorityId, string RoadNumber);

        Task<IEnumerable<Road>> RoadNumberAjaxListAsync(string RoadNumber);

        void Update(Road road);

        void Update(long ID, Road road);
        void Remove(Road road);

        Task DetachFirstEntryAsync(Road road);
    }
}
