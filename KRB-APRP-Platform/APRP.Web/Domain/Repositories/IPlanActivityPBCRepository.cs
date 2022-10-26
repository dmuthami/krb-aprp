using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IPlanActivityPBCRepository
    {
        Task<IEnumerable<PlanActivityPBC>> ListAsync();
        Task AddAsync(PlanActivityPBC planActivityPBC);
        Task<PlanActivityPBC> FindByIdAsync(long ID);
        void Update(PlanActivityPBC planActivityPBC); 
        void Remove(PlanActivityPBC planActivityPBC);
    }
}
