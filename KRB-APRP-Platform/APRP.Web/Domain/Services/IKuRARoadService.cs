using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services
{
    public interface IKuRARoadService
    {
        Task<KuRRARoadResponse> FindByIdAsync(long ID);

        Task<KuRRARoadResponse> FindByRoadNumberAsync(string RoadNumber);

        Task<KuRRARoadListResponse> ListAsync();

        Task<KuRRARoadListResponse> ListAsync(string RoadNumber);

        Task<KenHARoadDictResponse> ListByRoadClassAsync();

        Task<KuraRoadViewModelResponse> ListViewAsync();

        Task<KuRRARoadResponse> AddAsync(KuraRoad kuraRoad);

        Task<KuRRARoadResponse> UpdateAsync(long ID, KuraRoad kuraRoad);

        Task<KuRRARoadResponse> RemoveAsync(long ID);
    }
}
