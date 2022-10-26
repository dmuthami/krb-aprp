namespace APRP.Services.AuthorityAPI.Services
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
