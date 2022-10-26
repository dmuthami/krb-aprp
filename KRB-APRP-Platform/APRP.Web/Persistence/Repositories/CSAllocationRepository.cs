using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class CSAllocationRepository : BaseRepository, ICSAllocationRepository
    {
        private readonly ILogger _logger;
        public CSAllocationRepository(AppDbContext context, ILogger<CSAllocationRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(CSAllocation cSAllocation)
        {
            await _context.CSAllocations.AddAsync(cSAllocation).ConfigureAwait(false);
        }

        public async Task DetachFirstEntryAsync(CSAllocation cSAllocation)
        {
            _context.Entry(cSAllocation).State = EntityState.Detached;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> CSAllocationSummaryAsync(long FinancialYearId)
        {
            try
            {
                var DisbursementSummary = await Task.Run(() =>
                {
                    var x = from b in _context.CSAllocations
                            .AsEnumerable()
                            .Where(w => w.BudgetCeilingComputation.FinancialYearId == FinancialYearId)
                            group b by new { b.BudgetCeilingComputation.FinancialYearId, b.AuthorityId } into g
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
                _logger.LogError(Ex, $"CSAllocationtRepository.CSAllocationSummaryByBudgetCeilingComputationAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> CSAllocationSummaryByBudgetCeilingComputationAsync(long FinancialYearId)
        {
            try
            {
                var DisbursementSummary = await Task.Run(() =>
                {
                    var x = from b in _context.CSAllocations
                            .AsEnumerable()
                            .Where(w => w.BudgetCeilingComputation.FinancialYearId == FinancialYearId)
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
                _logger.LogError(Ex, $"CSAllocationtRepository.CSAllocationSummaryByBudgetCeilingComputationAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public async Task<CSAllocation> FindByCSAllocationEntryAsync(CSAllocation cSAllocation)
        {
            return await _context.CSAllocations
            .Where(d => d.BudgetCeilingComputationId == cSAllocation.BudgetCeilingComputationId
            && d.AuthorityId == cSAllocation.AuthorityId)
            .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<CSAllocation> FindByIdAsync(long ID)
        {
            return await _context.CSAllocations
            .Include(d => d.BudgetCeilingComputation)
            .Include(a => a.Authority)           
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<IEnumerable<CSAllocation>> ListAsync()
        {
            return await _context.CSAllocations
                            .Include(d => d.BudgetCeilingComputation)
                            .Include(a => a.Authority)
                            .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<CSAllocation>> ListAsync(long FinancialYearId)
        {
            return await _context.CSAllocations
                    .Where(w => w.BudgetCeilingComputation.FinancialYearId == FinancialYearId)
                    .Include(a => a.Authority)
                    .Include(k => k.BudgetCeilingComputation)
                    .ToListAsync().ConfigureAwait(false);

        }

        public async Task<IEnumerable<CSAllocation>> ListAsync(long AuthorityId, long FinancialYearId)
        {
            return await _context.CSAllocations
                    .Where(w => w.BudgetCeilingComputation.FinancialYearId == FinancialYearId && w.AuthorityId == AuthorityId)
                    .Include(a => a.Authority)
                    .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(CSAllocation cSAllocation)
        {
            _context.CSAllocations.Remove(cSAllocation);
        }

        public void Update(CSAllocation cSAllocation)
        {
            _context.CSAllocations.Update(cSAllocation);
        }

        public void Update(long ID, CSAllocation cSAllocation)
        {
            _context.Entry(cSAllocation).State = EntityState.Modified;
        }
    }
}
