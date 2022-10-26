using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IARICSRepository
    {
        Task<IEnumerable<ARICS>> ListAsync();
        Task AddAsync(ARICS aRICS);
        Task<ARICS> FindByIdAsync(long ID);
        void Update(ARICS aRICS);
        void Update(long ID, ARICS aRICS);

        void Remove(ARICS aRICS);
        Task<IList<ARICS>> GetARICSBySheetNo(long ID);
        Task<IList<ARICS>> GetARICSForSheetNo(long SheetID);
        Task<ARICS> CheckARICSForSheet(long SheetId);

        Task<ARICS> GetARICSDetails(long ID);

        Task<ARICS> FindByRoadSheetAndChainageAsync(long RoadSheetID, int Chainage);

        Task<IEnumerable<ARICS>> GetARICSForRoad(Road road, int? Year);

        Task<IEnumerable<ARICS>> GetARICSForRoad(Road road, double StartChainage, double EndChainage, int? Year);

        Task<IEnumerable<ARICS>> GetARICS(int? Year);

        Task<IQueryable<ARICS>> GetARICS2(Authority authority, int? Year);

        Task<IEnumerable<ARICS>> GetARICSByRoadSection(long RoadSectionId);

        Task<IActionResult> GetARICEDRoadSectionByAuthorityAndYear(long AuthorityId, int ARICSYear);
    }
}
