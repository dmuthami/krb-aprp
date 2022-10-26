using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class SubmitNewWorkplanModel
    {
        public FinancialYear FinancialYear { get; set; }
        public FundingSource FundingSource { get; set; }
        public ExecutionMethod ExecutionMethod { get; set; }
        public WorkCategory WorkCategory { get; set; }
        public Authority Authority { get; set; }
        public County County { get; set; }
        public Region Region { get; set; }
        public Contract Contract{ get; set; }
    }
}
