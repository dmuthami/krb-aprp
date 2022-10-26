using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services
{
    public interface IKenHARoadService
    {
        Task<KenHARoadResponse> FindByIdAsync(long ID);

        Task<KenHARoadResponse> FindByRoadNumberAsync(string RoadNumber);

        Task<KenHARoadResponse> FindBySectionIdAsync(string SectionID);

        Task<KenHARoadListResponse> ListAsync();

        Task<KenHARoadListResponse> ListAsync(string RoadNumber);

        Task<KenHARoadDictResponse> ListByRoadClassAsync();

        Task<KenhaRoadViewModelResponse> ListViewAsync();

        Task<KenHARoadResponse> AddAsync(KenhaRoad kenhaRoad);

        Task<KenHARoadResponse> UpdateAsync(long ID, KenhaRoad kenhaRoad);

        Task<KenHARoadResponse> RemoveAsync(long ID);
    }
}
