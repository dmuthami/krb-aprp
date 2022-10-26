using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IExecutionMethodRepository
    {
        Task<IEnumerable<ExecutionMethod>> ListAsync();
    }
}
