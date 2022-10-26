using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IShoulderInterventionPavedRepository
    {
        Task<IEnumerable<ShoulderInterventionPaved>> ListAsync();
        Task AddAsync(ShoulderInterventionPaved shoulderInterventionPaved);
        Task<ShoulderInterventionPaved> FindByIdAsync(long ID);
        void Update(ShoulderInterventionPaved shoulderInterventionPaved);
        void Remove(ShoulderInterventionPaved shoulderInterventionPaved);
    }
}
