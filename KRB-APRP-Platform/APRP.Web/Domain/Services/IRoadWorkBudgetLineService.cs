using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadWorkBudgetLineService
    {
        Task<IEnumerable<RoadWorkBudgetLine>> ListAsync(long HeaderId);

        Task<RoadWorkBudgetLineResponse> AddAsync(RoadWorkBudgetLine roadWorkBudgetLine);
        Task<RoadWorkBudgetLineResponse> FindByIdAsync(long ID);

        Task<RoadWorkBudgetLineResponse> FindByRoadWorkBudgetHeaderIdAsync(long RoadWorkBudgetHeaderId);

        Task<RoadWorkBudgetLineResponse> UpdateAsync(RoadWorkBudgetLine roadWorkBudgetLine);
        Task<RoadWorkBudgetLineResponse> RemoveAsync(long ID); 
    }
}
