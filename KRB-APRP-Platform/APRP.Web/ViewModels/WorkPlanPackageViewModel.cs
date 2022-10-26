using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class WorkPlanPackageViewModel
    {
        public IEnumerable<WorkPlanPackage> WorkPlanPackages { get; set; }
        public WorkPlanPackage WorkPlanPackage { get; set; }
        public Authority Authority { get; set; }

    }
}
