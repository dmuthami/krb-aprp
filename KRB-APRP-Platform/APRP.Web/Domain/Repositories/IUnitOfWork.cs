namespace APRP.Web.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
        Task ApplicationRoleCompleteAsync();
    }
}
