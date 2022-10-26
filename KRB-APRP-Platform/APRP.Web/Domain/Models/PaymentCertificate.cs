using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class PaymentCertificate
    {
        [Key]
        public long ID { get; set; }
        public int CertificateNo { get; set; }
        public int CertificateStatus { get; set; }
        public long ContractId { get; set; }
        public Contract Contract { get; set; }
        public double CertificateContractSum { get; set; }
        public double CertificateReviseContractSum { get; set; }
        public double CertificateValuationAsAt { get; set; }
        //work done
        public double CertificateTotalWorkDonePrevious { get; set; }
        public double CertificateTotalWorkDoneThis { get; set; }
        public double CertificateTotalWorkDoneTotals { get; set; }

        //Material On Site
        public double CertificateMaterialOnSitePrevious { get; set; }
        public double CertificateMaterialOnSiteThis { get; set; }
        public double CertificateMaterialOnSiteTotals { get; set; }

        //Variation of price
        public double CertificateVariationOfPricePrevious { get; set; }
        public double CertificateVariationOfPriceThis { get; set; }
        public double CertificateVariationOfPriceTotals { get; set; }

        //Rates
        public float VAT { get; set; }
        public float LessVAT { get; set; }
        public float Retention { get; set; }
        public float WithholdingTax { get; set; }

        //Advance
        public double AdvanceRecovery { get; set; }
        public double AdvanceBalance { get; set; }
        public double LateRepaymentsInterest { get; set; }
        public double LiquidatedDamages { get; set; }

        //Signing and Verification
        public string SignedBy { get; set; }
        public DateTime SignedByDatetime { get; set; }
        public string EmployerRepresentativeSign { get; set; }
        public DateTime EmployerRepresentativeSignDatetime { get; set; }
        public string EngineerSign { get; set; }
        public DateTime EngineerSignDatetime { get; set; }

        //net
        public double NetPaymentPrevious { get; set; }
        public double NetPaymentThis { get; set; }
        public double NetPaymentTotals { get; set; }

        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }

        public IEnumerable<PaymentCertificateActivity> PaymentCertificateActivities { get; }

    }
}
