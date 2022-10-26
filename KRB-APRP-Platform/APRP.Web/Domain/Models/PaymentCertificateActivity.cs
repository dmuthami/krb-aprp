using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class PaymentCertificateActivity
    {
        [Key]
        public long ID { get; set; }
        public long PaymentCertificateId { get; set; }
        public PaymentCertificate PaymentCertificate { get; set; }
        public long PlanActivityId { get; set; }
        public PlanActivity PlanActivity { get; set; }
        public int Quantity { get; set; }

        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
