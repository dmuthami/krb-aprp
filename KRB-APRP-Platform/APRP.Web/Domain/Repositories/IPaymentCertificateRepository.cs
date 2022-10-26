using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IPaymentCertificateRepository
    {
        Task<IEnumerable<PaymentCertificate>> ListAsync(long contractId);
        Task AddAsync(PaymentCertificate paymentCertificate);

        void Update(PaymentCertificate paymentCertificate);
        Task<PaymentCertificate> FindByIdAsync(long ID);
        Task<PaymentCertificate> FindByContractIdAndCertificateNo(long contractId, int currentCertificateNo);
    }
}
