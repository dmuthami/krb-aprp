using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services
{
    public interface IMonthCodeService
    {
        Task<IEnumerable<MonthCode>> ListAsync();

    }
}
