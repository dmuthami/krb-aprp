using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class PaymentCertificateRepository : BaseRepository, IPaymentCertificateRepository
    {
        public PaymentCertificateRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(PaymentCertificate paymentCertificate)
        {
            await _context.PaymentCertificates.AddAsync(paymentCertificate);
        }

        public async Task<PaymentCertificate> FindByIdAsync(long ID)
        {
            return await _context.PaymentCertificates
                .Include(c => c.Contract).ThenInclude(w => w.WorkPlanPackage).ThenInclude(f=>f.FinancialYear)
                .Include(c => c.Contract).ThenInclude(c => c.Contractor)
                .Include(e=>e.PaymentCertificateActivities).ThenInclude(p=>p.PlanActivity).ThenInclude(i=>i.ItemActivityUnitCost).ThenInclude(g=>g.ItemActivityGroup)
                .FirstOrDefaultAsync(i => i.ID == ID).ConfigureAwait(false);
        }

        public void Update(PaymentCertificate paymentCertificate)
        {
            _context.PaymentCertificates.Update(paymentCertificate);
        }

        public async Task<PaymentCertificate> FindByContractIdAndCertificateNo(long contractId, int currentCertificateNo)
        {
            return await _context.PaymentCertificates
                .FirstOrDefaultAsync(i => i.ContractId == contractId && i.CertificateNo == currentCertificateNo).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PaymentCertificate>> ListAsync(long contractId)
        {
            return await _context.PaymentCertificates.Where(i=>i.ContractId == contractId).ToListAsync().ConfigureAwait(false);
        }

    }
}
