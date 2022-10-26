using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class AuthorityViewModel2
    {
        public IEnumerable<Authority>? PagedData { get; set; } = Enumerable.Empty<Authority>();

        public int TotalRecords { get; set; }
    }
}
