using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class RevenueCollectionViewModel
    {
        public Authority Authority { get; set; }

        public FinancialYear FinacialYear { get; set; }
        public FinancialYear FinancialYear { get; set; }

        public IList<RevenueCollection> RevenueCollection { get; set; }

        public double RevenueCollectionSum { get; set; }

        public IList<Disbursement> Disbursement { get; set; }
        public double DisbursementSum { get; set; }

        public IList<Allocation> Allocations { get; set; }

        public IList<OtherFundItem> OtherFundItem { get; set; }

        public int ApprovalStatus { get; set; } = 0;

        public string Referer { get; set; }

        public CeilingsEstimateViewModel CeilingsEstimateViewModel { get; set; }

    }
}
