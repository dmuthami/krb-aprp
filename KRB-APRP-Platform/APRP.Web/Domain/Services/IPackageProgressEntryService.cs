using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IPackageProgressEntryService
    {
        Task<IEnumerable<PackageProgressEntry>> ListAsync();

        Task<PackageProgressEntryResponse> AddAsync(PackageProgressEntry packageProgressEntry);
    }
}
