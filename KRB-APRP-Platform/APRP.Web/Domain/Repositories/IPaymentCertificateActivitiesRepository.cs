using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IPaymentCertificateActivitiesRepository
    {
        Task AddAsync(PaymentCertificateActivity paymentCertificateActivity);
        Task<PaymentCertificateActivity> FindByIdAsync(long ID);
        void Update(PaymentCertificateActivity paymentCertificateActivity);
        void Remove(PaymentCertificateActivity paymentCertificateActivity);
       
    }
}
