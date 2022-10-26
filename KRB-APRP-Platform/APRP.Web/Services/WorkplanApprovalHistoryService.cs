using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class WorkplanApprovalHistoryService : IWorkplanApprovalHistoryService
    {
        private readonly IWorkplanApprovalHistoryRepository _workplanApprovalHistoryRepository;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;

        public WorkplanApprovalHistoryService(IWorkplanApprovalHistoryRepository workplanApprovalHistoryRepository
            , IUnitOfWork unitOfWork
            , ILogger<WorkplanApprovalHistoryService> logger)
        {
            _workplanApprovalHistoryRepository = workplanApprovalHistoryRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<WorkplanApprovalHistoryResponse> AddAsync(WorkplanApprovalHistory workplanApprovalHistory)
        {
            try
            {

                await _workplanApprovalHistoryRepository.AddAsync(workplanApprovalHistory).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new WorkplanApprovalHistoryResponse(workplanApprovalHistory);
            }
            catch (Exception ex)
            {
                return new WorkplanApprovalHistoryResponse($"Error occured while saving the record {ex.Message}");
            }
        }
    }
}
