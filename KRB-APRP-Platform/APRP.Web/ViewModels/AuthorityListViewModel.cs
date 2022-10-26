using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class AuthorityListViewModel
    {
        public IEnumerable<Authority> Authorities { get; set; }
        public IEnumerable<Region> Regions { get; set; }
        public IEnumerable<Constituency> Constituencies { get; set; }
    }
}
