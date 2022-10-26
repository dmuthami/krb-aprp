using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class WorkPlanPackageContractViewModel
    {
        public IEnumerable<Contract> Contracts { get; set; }
        public Contract Contract { get; set; }
        public Authority Authority { get; set; }
    }
}
