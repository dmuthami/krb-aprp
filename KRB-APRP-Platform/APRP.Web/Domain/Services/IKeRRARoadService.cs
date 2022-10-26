using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services
{
    public interface IKeRRARoadService
    {
        Task<KeRRARoadResponse> FindByIdAsync(long ID);

        Task<KeRRARoadResponse> FindByRoadNumberAsync(string RoadNumber);

        Task<KeRRARoadResponse> FindBySectionIdAsync(string SectionID);

        Task<KeRRARoadListResponse> ListAsync();

        Task<KerraRoadViewModelResponse> ListViewAsync();

        Task<KeRRARoadListResponse> ListAsync(string RoadNumber);

        Task<KenHARoadDictResponse> ListByRoadClassAsync();

        Task<KeRRARoadResponse> AddAsync(KerraRoad kerraRoad);

        Task<KeRRARoadResponse> UpdateAsync(long ID, KerraRoad kerraRoad);

        Task<KeRRARoadResponse> RemoveAsync(long ID);
    }
}
