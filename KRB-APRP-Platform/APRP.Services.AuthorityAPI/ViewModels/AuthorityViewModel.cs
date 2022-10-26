using APRP.Services.AuthorityAPI.Models;

namespace APRP.Services.AuthorityAPI.ViewModels
{
    public class AuthorityViewModel
    {
        public IEnumerable<Authority>? PagedData { get; set; }=Enumerable.Empty<Authority>();   

        public int TotalRecords { get; set; }
    }
}
