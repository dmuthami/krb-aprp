using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IQuarterCodeListRepository
    {
        Task<IEnumerable<QuarterCodeList>> ListAsync();

        Task AddAsync(QuarterCodeList quarterCodeList);
        Task<QuarterCodeList> FindByIdAsync(long ID);

        Task<QuarterCodeList> FindByNameAsync(string Item);

        void Update(QuarterCodeList quarterCodeList);
        void Update(long ID, QuarterCodeList quarterCodeList);

        void Remove(QuarterCodeList quarterCodeList);
    }
}
