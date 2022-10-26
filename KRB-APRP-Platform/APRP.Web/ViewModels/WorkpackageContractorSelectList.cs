using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class WorkpackageContractorSelectList
    {
        public long contractId { get; set; }
        public IEnumerable<Contractor> Contractors { get; set; }
    }
}
