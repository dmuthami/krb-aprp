using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IQuarterCodeUnitRepository
    {
        Task<IEnumerable<QuarterCodeUnit>> ListAsync();

        Task<IEnumerable<QuarterCodeUnit>> ListAsync(long FinancialYearId);

        Task AddAsync(QuarterCodeUnit quarterCodeUnit);
        Task<QuarterCodeUnit> FindByIdAsync(long ID);

        void Update(QuarterCodeUnit quarterCodeUnit);
        void Update(long ID, QuarterCodeUnit quarterCodeUnit);

        void Remove(QuarterCodeUnit quarterCodeUnit);

        Task<QuarterCodeUnit> FindByQuarterCodeListIdAndFinancialIdAsync(long QuarterCodeListId, long FinancialYearId);
    }
}
