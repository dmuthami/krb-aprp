using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class PaymentCertificateActivityResponse : BaseResponse
    {
        public PaymentCertificateActivity PaymentCertificateActivity{ get; set; }
        public PaymentCertificateActivityResponse(bool success, string message, PaymentCertificateActivity paymentCertificateActivity) : base(success, message)
        {
            PaymentCertificateActivity = paymentCertificateActivity;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="paymentCertificateActivity">Saved road.</param>
        /// <returns>Response.</returns>
        public PaymentCertificateActivityResponse(PaymentCertificateActivity paymentCertificateActivity) : this(true, string.Empty, paymentCertificateActivity)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public PaymentCertificateActivityResponse(string message) : this(false, message, null) 
        { }
    }
}
