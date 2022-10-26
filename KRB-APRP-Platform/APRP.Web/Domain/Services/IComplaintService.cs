using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IComplaintService
    {
        Task<IEnumerable<Complaint>> ListAsync();
        Task<IEnumerable<Complaint>> ListUnresolvedAsyc();
        Task<ComplaintResponse> AddAsync(Complaint complaint);
        Task<ComplaintResponse> FindByIdAsync(long ID);
        Task<ComplaintResponse> UpdateAsync(Complaint complaint);
        Task<ComplaintResponse> RemoveAsync(long ID);
    }
}