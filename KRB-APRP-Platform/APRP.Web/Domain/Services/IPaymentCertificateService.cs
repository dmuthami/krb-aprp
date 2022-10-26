using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IPaymentCertificateService
    {
        Task<IEnumerable<PaymentCertificate>> ListAsync(long contractId);

        Task<PaymentCertificateResponse> AddAsync(PaymentCertificate paymentCertificate);

        Task<PaymentCertificateResponse> UpdateAsync(PaymentCertificate paymentCertificate);
        Task<PaymentCertificateResponse> FindByIdAsync(long certificateId);
        Task<PaymentCertificateResponse> FindByContractIdAndCertificateNo(long contractId, int currentCertificateNo);
    }
}
