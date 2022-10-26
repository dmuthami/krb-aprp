using APRP.Web.Domain.Services.Communication;
using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services
{
    public interface ICountiesRoadService
    {
        Task<CountiesRoadResponse> FindByIdAsync(long ID);

        Task<CountiesRoadResponse> FindByRoadNumberAsync(string RoadNumber);

        Task<CountiesRoadListResponse> ListAsync();

        Task<CountiesRoadListResponse> ListAsync(string RoadNumber);

        Task<KenHARoadDictResponse> ListByRoadClassAsync();

        Task<CountiesRoadViewModelResponse> ListViewAsync(long AuthorityId);

        Task<CountiesRoadResponse> AddAsync(CountiesRoad countiesRoad);

        Task<CountiesRoadResponse> UpdateAsync(long ID, CountiesRoad countiesRoad);

        Task<CountiesRoadResponse> RemoveAsync(long ID);
    }
}
