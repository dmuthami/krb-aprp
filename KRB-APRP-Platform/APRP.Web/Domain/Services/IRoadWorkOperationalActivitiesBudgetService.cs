using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadWorkOperationalActivitiesBudgetService
    {
        Task<IEnumerable<RoadWorkOperationalActivitiesBudget>> ListAsync(long HeaderId);

        Task<RoadWorkOperationalActivitiesBudgetResponse> AddAsync(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget);
        Task<RoadWorkOperationalActivitiesBudgetResponse> FindByIdAsync(long ID);
        Task<RoadWorkOperationalActivitiesBudgetResponse> UpdateAsync(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget);
        Task<RoadWorkOperationalActivitiesBudgetResponse> RemoveAsync(long ID); 
    }
}
