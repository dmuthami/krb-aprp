using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IOtherFundItemRepository
    {
        Task<IEnumerable<OtherFundItem>> ListAsync();

        Task<IEnumerable<OtherFundItem>> ListAsync(long FinancialYearId);

        Task AddAsync(OtherFundItem otherFundItem);
        Task<OtherFundItem> FindByIdAsync(long ID);

        Task<OtherFundItem> FindByNameAsync(string Description);

        void Update(OtherFundItem otherFundItem);
        void Update(long ID, OtherFundItem otherFundItem);

        void Remove(OtherFundItem otherFundItem);

        Task<OtherFundItem> FindByFinancialIdAsync(long FinancialYearId);
    }
}
