using APRP.Services.AuthorityAPI.Filter;

namespace APRP.Services.AuthorityAPI.Services
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
