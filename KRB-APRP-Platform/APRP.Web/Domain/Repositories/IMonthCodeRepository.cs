using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IMonthCodeRepository
    {
        Task<IEnumerable<MonthCode>> ListAsync();
    }
}
