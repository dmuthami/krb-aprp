using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IPlanActivityRepository
    {
        Task<IEnumerable<PlanActivity>> ListAsync();
        Task AddAsync(PlanActivity planActivity);
        Task<PlanActivity> FindByIdAsync(long ID);
        void Update(PlanActivity planActivity); 
        void Remove(PlanActivity planActivity);
    }
}
