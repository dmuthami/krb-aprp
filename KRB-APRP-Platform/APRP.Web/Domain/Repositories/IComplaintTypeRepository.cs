using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IComplaintTypeRepository
    {
        Task<IEnumerable<ComplaintType>> ListAsync();

    }
}
