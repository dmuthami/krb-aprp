using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IEmploymentDetailRepository
    {
        Task<IEnumerable<EmploymentDetail>> ListAsync();
        Task AddAsync(EmploymentDetail employmentDetail);

        void Update(EmploymentDetail employmentDetail);
        void Remove(EmploymentDetail employmentDetail);
        Task<EmploymentDetail> FindByIdAsync(long ID);
    }
}
