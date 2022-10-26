using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services
{
    public interface IExecutionMethodService
    {
        Task<IEnumerable<ExecutionMethod>> ListAsync();
    }
}
