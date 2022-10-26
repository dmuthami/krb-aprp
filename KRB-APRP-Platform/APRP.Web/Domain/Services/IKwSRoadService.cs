using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services
{
    public interface IKwSRoadService
    {
        Task<KwSRoadResponse> FindByIdAsync(long ID);

        Task<KwSRoadResponse> FindByRoadNumberAsync(string RoadNumber);

        Task<KwSRoadResponse> FindBySectionIdAsync(string SectionID);

        Task<KwSRoadListResponse> ListAsync();

        Task<KwSRoadListResponse> ListAsync(string RoadNumber);

        Task<KwsRoadViewModelResponse> ListViewAsync();

        Task<KwSRoadResponse> AddAsync(KwsRoad kwsRoad);

        Task<KwSRoadResponse> UpdateAsync(long ID, KwsRoad kwsRoad);

        Task<KwSRoadResponse> RemoveAsync(long ID);
    }
}
