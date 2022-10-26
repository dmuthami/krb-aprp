using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class DisbursementRepository : BaseRepository, IDisbursementRepository
    {
        private readonly ILogger _logger;
        public DisbursementRepository(AppDbContext context, ILogger<DisbursementRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(Disbursement disbursement)
        {
            await _context.Disbursements.AddAsync(disbursement).ConfigureAwait(false);
        }

        public async Task DetachFirstEntryAsync(Disbursement disbursement)
        {
            _context.Entry(disbursement).State = EntityState.Detached;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementSummaryAsync(long FinancialYearId)
        {
            try
            {
                var DisbursementSummary = await Task.Run(() =>
                {
                    var x = from b in _context.Disbursements
                            .AsEnumerable()
                            .Where(w => w.FinancialYearId == FinancialYearId)
                            group b by new { b.FinancialYearId, b.AuthorityId } into g
                            select new
                            {
                                FinancialYearId = g.Key.FinancialYearId,
                                AuthorityId = g.Key.AuthorityId,
                                Count = g.Count(),
                                TotalDisbursement = g.Sum(n => n.Amount)
                            };
                    return x.ToList();
                }).ConfigureAwait(false);

                //IQueryable<DisbursementSummaryViewModel> objectList = (IQueryable<DisbursementSummaryViewModel>)DisbursementSummary;
                return Ok(DisbursementSummary);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementRepository.DisbursementSummaryByBudgetCeilingComputationAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementSummaryByBudgetCeilingComputationAsync(long FinancialYearId)
        {
            try
            {
                var DisbursementSummary = await Task.Run(() =>
                {
                    var x = from b in _context.Disbursements
                            .AsEnumerable()
                            .Where(w => w.FinancialYearId == FinancialYearId)
                            group b by new { b.BudgetCeilingComputationId } into g
                            select new
                            {
                                BudgetCeilingComputationId = g.Key.BudgetCeilingComputationId,
                                //AuthorityId = g.Key.AuthorityId,
                                Count = g.Count(),
                                TotalDisbursement = g.Sum(n => n.Amount)
                            };
                    return x.ToList();
                }).ConfigureAwait(false);

                return Ok(DisbursementSummary);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementRepository.DisbursementSummaryByBudgetCeilingComputationAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public async Task<Disbursement> FindByAuthorityIdAndFinancialYearIdAsync(long AuthorityId, long FinancialYearId)
        {
            return await _context.Disbursements
                .Where(x => x.AuthorityId == AuthorityId && x.FinancialYearId == FinancialYearId)
              .Include(d => d.FinancialYear)
              .Include(a => a.Authority)
              .Include(e => e.DisbursementTranche)
              .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<Disbursement> FindByDisbursementEntryAsync(Disbursement disbursement)
        {
            return await _context.Disbursements
            .Where(d => d.FinancialYearId == disbursement.FinancialYearId
            && d.AuthorityId == disbursement.AuthorityId &&
            d.BudgetCeilingComputationId == disbursement.BudgetCeilingComputationId
            && d.DisbursementTrancheId == disbursement.DisbursementTrancheId)
            .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<Disbursement> FindByIdAsync(long ID)
        {
            return await _context.Disbursements
            .Include(d => d.FinancialYear)
            .Include(a => a.Authority)
            .Include(e => e.DisbursementTranche)
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Disbursement>> ListAsync()
        {
            return await _context.Disbursements
                            .Include(d => d.FinancialYear)
                            .Include(a => a.Authority)
                            .Include(e => e.DisbursementTranche)
                            .Include(k => k.BudgetCeilingComputation)
                            .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Disbursement>> ListAsync(long FinancialYearId)
        {
            return await _context.Disbursements
                    .Where(w => w.FinancialYearId == FinancialYearId)
                    .Include(d => d.FinancialYear)
                    .Include(a => a.Authority)
                    .Include(e => e.DisbursementTranche)
                    .Include(k => k.BudgetCeilingComputation)
                    .Include(a=>a.DisbursementReleases)
                    .ToListAsync().ConfigureAwait(false);

        }

        public async Task<IEnumerable<Disbursement>> ListAsync(long AuthorityId, long FinancialYearId)
        {
            return await _context.Disbursements
                    .Where(w => w.FinancialYearId == FinancialYearId && w.AuthorityId == AuthorityId)
                    .Include(d => d.FinancialYear)
                    .Include(a => a.Authority)
                    .Include(e => e.DisbursementTranche)
                    .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Disbursement>> ListDisbursementByReleaseAsync(long ReleaseId)
        {
            IEnumerable<Disbursement> disbursementEnum = new List<Disbursement>();
            IList<Disbursement> disbursements = (IList<Disbursement>)disbursementEnum;

            var disbursementList = await _context.Disbursements
                    .Include(d => d.DisbursementReleases)
                    .ToListAsync().ConfigureAwait(false);
            foreach (var disbursement in disbursementList)
            {
                var find = disbursement.DisbursementReleases
                    .Where(x => x.ReleaseId == ReleaseId).FirstOrDefault();
                if (find != null)
                {
                    disbursements.Add(disbursement);
                }
            }
            return disbursements;
        }

        public void Remove(Disbursement disbursement)
        {
            _context.Disbursements.Remove(disbursement);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> SummarizeByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync(long FinancialYearId)
        {
            try
            {
                var DisbursementSummary = await Task.Run(() =>
                {
                    var x = from b in _context.Disbursements
                            .AsEnumerable()
                            .Where(w => w.FinancialYearId == FinancialYearId)
                            group b by new { b.FinancialYearId, b.AuthorityId, b.DisbursementTrancheId } into g
                            select new
                            {
                                FinancialYearId = g.Key.FinancialYearId,
                                AuthorityId = g.Key.AuthorityId,
                                DisbursementTrancheId = g.Key.DisbursementTrancheId,
                                Count = g.Count(),
                                DisbursementTrancheAmount = g.Sum(n => n.Amount)
                            };
                    return x.ToList();
                }).ConfigureAwait(false);

                //IQueryable<DisbursementSummaryViewModel> objectList = (IQueryable<DisbursementSummaryViewModel>)DisbursementSummary;
                return Ok(DisbursementSummary);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementRepository.SummarizeByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public void Update(Disbursement disbursement)
        {
            _context.Disbursements.Update(disbursement);
        }

        public void Update(long ID, Disbursement disbursement)
        {
            _context.Entry(disbursement).State = EntityState.Modified;
        }
    }
}
