using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IAllocationRepository
    {
        Task<IEnumerable<Allocation>> ListAsync();

        Task<IEnumerable<Allocation>> ListAsync(long FinancialYearId);
        Task AddAsync(Allocation allocation);
        Task<Allocation> FindByIdAsync(long ID);
        Task<Allocation> FindByAllocationCodeUnitIdAsync(long AllocationCodeUnitId);
        void Update(Allocation allocation);
        void Update(long ID, Allocation allocation);
        void Remove(Allocation allocation);
    }
}
