using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadService
    {
        Task<RoadListResponse> ListAsync();

        Task<RoadViewResponse> ListViewAsync();
        Task<RoadViewResponse> ListViewAsync(Authority authority);
        Task<RoadViewWithARICSResponse> ListViewWithAricsAsync(int? Year);
        Task<RoadViewWithARICSResponse> ListViewWithAricsAsync(Authority authority, int? Year);
        Task<RoadListResponse> ListAsync(Authority authority);

        Task<RoadListResponse> GetRoadWithSectionsAsync(Authority authority, string SurfaceType);

        Task<RoadListResponse> ListByNameAsync(string RoadName);
        Task<RoadResponse> AddAsync(Road road);
        Task<RoadResponse> FindByDisbursementEntryAsync(Road road);
        Task<RoadResponse> FindByIdAsync(long ID);

        Task<RoadResponse> FindByIdAsync(long ID, int ARICSYear);
        Task<RoadResponse> FindByRoadNumberAsync(string RoadNumber);
        Task<RoadResponse> FindByRoadNumberAsync(long AuthorityId,string RoadNumber);
        Task<RoadListResponse> RoadNumberAjaxListAsync(string RoadNumber);
        Task<RoadResponse> FindByNameAsyc(string RoadName);
        Task<RoadResponse> UpdateAsync(Road road);
        Task<RoadResponse> UpdateAsync(long ID, Road road);
        Task<RoadResponse> RemoveAsync(long ID);
        Task<RoadResponse> DetachFirstEntryAsync(Road road);
    }
}
