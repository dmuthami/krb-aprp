namespace APRP.Web.Domain.Models
{
    public class DisbursementRelease
    {
        public long DisbursementId { get; set; }
        public Disbursement Disbursement { get; set; }

        public long ReleaseId { get; set; }
        public Release Release { get; set; }
    }
}
