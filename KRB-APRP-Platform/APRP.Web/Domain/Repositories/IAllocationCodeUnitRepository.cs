using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IAllocationCodeUnitRepository
    {
        Task<IEnumerable<AllocationCodeUnit>> ListAsync(string AuthorityType);

        Task AddAsync(AllocationCodeUnit roadClassCodeUnit);
        Task<AllocationCodeUnit> FindByIdAsync(long ID);

        Task<AllocationCodeUnit> FindByNameAsync(string Item);

        void Update(AllocationCodeUnit roadClassCodeUnit);
        void Update(long ID, AllocationCodeUnit roadClassCodeUnit);

        void Remove(AllocationCodeUnit roadClassCodeUnit);

        Task<AllocationCodeUnit> FindByAuthorityAsync(long AuthorityId);
    }
}
