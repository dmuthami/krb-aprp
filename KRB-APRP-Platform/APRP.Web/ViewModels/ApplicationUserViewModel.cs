using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class ApplicationUserViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }

        public ApplicationRole ApplicationRole { get; set; }

        public IEnumerable<ApplicationRole> ApplicationRoles { get; set; }

        public IEnumerable<ApplicationRole> MyGroups { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public bool Add { get; set; }

        public IList<ApplicationUser> applicationUsers { get; set; }

    }
}
