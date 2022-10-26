using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IPackageProgressEntryRepository
    {
        Task<IEnumerable<PackageProgressEntry>> ListAsync();
        Task AddAsync(PackageProgressEntry packageProgressEntry);
    }
}
