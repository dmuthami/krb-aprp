using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services
{
    public interface IComplaintTypeService
    {
        Task<IEnumerable<ComplaintType>> ListAsync();

    }
}
