using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class PaymentCertificateResponse : BaseResponse
    {
        public PaymentCertificate PaymentCertificate{ get; set; }
        public PaymentCertificateResponse(bool success, string message, PaymentCertificate paymentCertificate) : base(success, message)
        {
            PaymentCertificate = paymentCertificate;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="paymentCertificate">Saved road.</param>
        /// <returns>Response.</returns>
        public PaymentCertificateResponse(PaymentCertificate paymentCertificate) : this(true, string.Empty, paymentCertificate)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public PaymentCertificateResponse(string message) : this(false, message, null) 
        { }
    }
}
