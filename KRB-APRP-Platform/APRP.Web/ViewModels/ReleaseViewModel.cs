using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class ReleaseViewModel
    {
        public Release Release { get; set; }

        public IEnumerable<Release> Releases { get; set; }

        public string Referer { get; set; }

        public FinancialYear FinancialYear { get; set; }

        public long FinancialYearId { get; set; }

        public Disbursement Disbursement { get; set; }

        public IEnumerable<Disbursement> Disbursements { get; set; }

        public IEnumerable<Disbursement> ReleaseDisbursements { get; set; }

    }
}
