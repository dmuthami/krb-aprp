using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;

namespace APRP.Web.Services
{
    public class ExecutionMethodService : IExecutionMethodService
    {
        private readonly IExecutionMethodRepository _executionMethodRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExecutionMethodService(IExecutionMethodRepository executionMethodRepository, IUnitOfWork unitOfWork)
        {
            _executionMethodRepository = executionMethodRepository;
            _unitOfWork = unitOfWork;

        }
        public async Task<IEnumerable<ExecutionMethod>> ListAsync()
        {
            return await _executionMethodRepository.ListAsync().ConfigureAwait(false);
        }
    }
}
