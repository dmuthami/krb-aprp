using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IUploadRepository
    {
        Task<IEnumerable<Upload>> ListAsync();

        Task<IEnumerable<Upload>> ListAsync(string Type, long ForeignId);

        Task AddAsync(Upload aRICS);
        Task<Upload> FindByIdAsync(long ID);
        void Update(Upload upload);
        void Remove(Upload upload);
    }
}
