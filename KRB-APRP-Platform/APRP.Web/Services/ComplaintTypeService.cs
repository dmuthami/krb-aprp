using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;

namespace APRP.Web.Services
{
    public class ComplaintTypeService : IComplaintTypeService
    {
        private readonly IComplaintTypeRepository _complaintTypeRepository;

        public ComplaintTypeService(IComplaintTypeRepository complaintTypeRepository)
        {
            _complaintTypeRepository = complaintTypeRepository;

        }
        public async Task<IEnumerable<ComplaintType>> ListAsync()
        {
            return await _complaintTypeRepository.ListAsync().ConfigureAwait(false);
        }
    }
}
