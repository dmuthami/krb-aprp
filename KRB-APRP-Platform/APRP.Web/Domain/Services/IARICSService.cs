using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Domain.Services
{
    public interface IARICSService
    {
        Task<ARICSListResponse> ListAsync();
        Task<ARICSResponse> AddAsync(ARICS aRICS);
        Task<ARICSResponse> FindByIdAsync(long ID);
        Task<ARICSResponse> Update(ARICS aRICS);
        Task<ARICSResponse> Update(long ID, ARICS aRICS);
        Task<ARICSResponse> RemoveAsync(long ID);

        Task<ARICSListResponse> GetARICSBySheetNo(ARICSVM _ARICSVM);

        Task<ARICSResponse> CheckARICSForSheet(ARICSVM _ARICSVM);

        Task<ARICSListResponse> CreateARICSForSheet(ARICSVM _ARICSVM);

        Task<ARICSListResponse> GetARICSForSheet(ARICSVM _ARICSVM);

        Task<ARICSResponse> GetARICSDetails(long ID);

        Task<ARICSResponse> FindByRoadSheetAndChainageAsync(long RoadSheetID, int Chainage);

        Task<ARICSListResponse> GetARICSForRoad(Road road, int? Year);

        Task<ARICSListResponse> GetARICSForRoad(Road road,double StartChainage, double EndChainage, int? Year);

        /// <summary>
        /// Get the roads that ARICS has been conducted on...
        /// </summary>
        /// <param name="road"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        Task<RoadListResponse> GetARICEDRoads(int? Year);

        Task<RoadSectionViewModelResponse> GetARICEDRoadSection(Authority authority, int? Year);

        Task<RoadSectionListResponse> GetARICEDRoadSections(int? Year);

        Task<ARICSDataListResponse> GetARICEDRoadsAndConditions(int? Year);

        Task<ARICSDataResponse> GetIRI(IList<ARICS> roadARICS);

        Task<ARICSDataResponse> GetCulvertsSummaryForSheet(IList<ARICS> roadARICS);

        Task<ARICSDataResponse> GetIRIForRoad(Road road, int? Year);

        Task<ARICSDataListResponse> GetRoadsAndConditions(int? Year, Authority authority);

        Task<ARICSListResponse> GetARICSByRoadSection(long RoadSectionId);

        Task<GenericResponse> GetARICEDRoadSectionByAuthorityAndYear(long AuthorityId, int ARICSYear);
    }
}
