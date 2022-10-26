using APRP.Web.Domain.Models;
using System.ComponentModel;

namespace APRP.Web.ViewModels
{
    public class WorkplanUploadViewModel
    {
        public FinancialYear FinancialYear { get; set; }
        public FundingSource FundingSource { get; set; }
        [DisplayName("Workplan Document")]
        public IFormFile SupportDocument { get; set; }
    }
}
