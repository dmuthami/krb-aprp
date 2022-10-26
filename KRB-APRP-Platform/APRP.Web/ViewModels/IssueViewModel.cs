using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class IssueViewModel
    {
        public ApplicationUser  ApplcationUser{get; set; }
        public IEnumerable<Complaint> Complaints { get; set; }
    }
}
