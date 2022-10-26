using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class PaymentCertificateActivitiesRepository : BaseRepository, IPaymentCertificateActivitiesRepository
    {
        public PaymentCertificateActivitiesRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(PaymentCertificateActivity paymentCertificateActivity)
        {
            await _context.PaymentCertificateActivities.AddAsync(paymentCertificateActivity);
        }

        public async Task<PaymentCertificateActivity> FindByIdAsync(long ID)
        {
            return await _context.PaymentCertificateActivities
                .Include(c=>c.PaymentCertificate)
                .Include(p=>p.PlanActivity).ThenInclude(i=>i.ItemActivityUnitCost).ThenInclude(g=>g.ItemActivityGroup)
                .FirstOrDefaultAsync(i => i.ID == ID).ConfigureAwait(false);
        }

        public void Remove(PaymentCertificateActivity paymentCertificateActivity)
        {
            _context.Remove(paymentCertificateActivity);
        }

        public void Update(PaymentCertificateActivity paymentCertificateActivity)
        {
            _context.Update(paymentCertificateActivity);
        }
    }
}
