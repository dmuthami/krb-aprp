using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IARICSUploadRepository
    {
        Task<IEnumerable<ARICSUpload>> ListAsync();
        Task AddAsync(ARICSUpload aRICS);
        Task<ARICSUpload> FindByIdAsync(long ID);
        void Update(ARICSUpload aRICSUpload);
        void Remove(ARICSUpload aRICSUpload);
    }
}
