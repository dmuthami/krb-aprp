using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;

namespace APRP.Web.Services
{
    public class MonthCodeService : IMonthCodeService
    {
        private readonly IMonthCodeRepository _monthCodeRepository;
        public MonthCodeService(IMonthCodeRepository monthCodeRepository)
        {
            _monthCodeRepository = monthCodeRepository;
        }
        Task<IEnumerable<MonthCode>> IMonthCodeService.ListAsync()
        {
            return _monthCodeRepository.ListAsync();
        }
    }
}
