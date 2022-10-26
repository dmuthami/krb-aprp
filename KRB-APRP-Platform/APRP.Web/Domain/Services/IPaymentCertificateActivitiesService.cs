using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IPaymentCertificateActivitiesService
    {

        Task<PaymentCertificateActivityResponse> AddAsync(PaymentCertificateActivity paymentCertificateActivity);
        Task<PaymentCertificateActivityResponse> FindByIdAsync(long activityId);
        Task<PaymentCertificateActivityResponse> UpdateAsync(PaymentCertificateActivity paymentCertificateActivity);
        Task<PaymentCertificateActivityResponse> RemoveAsync(long ID);
    }
}
