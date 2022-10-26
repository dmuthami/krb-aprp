using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class ContractorViewModel
    {
        public IEnumerable<Contractor> Contractors { get; set; }
        public Contractor Contractor { get; set; }
    }
}
