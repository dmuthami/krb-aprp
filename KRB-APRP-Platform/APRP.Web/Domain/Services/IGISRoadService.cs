using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IGISRoadService
    {
        Task<IEnumerable<GISRoad>> ListAsync();
        Task<IEnumerable<GISRoad>> ListByNameAsync(string RoadName);
        Task<GISRoadResponse> AddAsync(GISRoad gISRoad);
        Task<GISRoadResponse> FindByIdAsync(long ID);
        Task<GISRoadResponse> FindByRoadNumberAsync(string RoadNumber);
        Task<GISRoadResponse> FindByNameAsyc(string RoadName);
        Task<GISRoadResponse> UpdateAsync(GISRoad gISRoad);
        Task<GISRoadResponse> RemoveAsync(long ID);
        Task<GISRoadResponse> GetRoadLengthAsync(string RoadNumber);
        Task<RoadResponse> PullRoadSectionFromGISAsync(long RoadID);
        Task<GISRoadViewModelResponse> GetSurfaceType(RoadSection roadSection);
    }
}
