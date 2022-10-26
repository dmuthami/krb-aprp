using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IComplaintRepository
    {
        Task<IEnumerable<Complaint>> ListAsync();
        Task<IEnumerable<Complaint>> ListUnresolvedAsyc();
        Task AddAsync(Complaint complaint);
        Task<Complaint> FindByIdAsync(long ID);
        void Update(Complaint complaint);
        void Remove(Complaint complaint);
    }
}
