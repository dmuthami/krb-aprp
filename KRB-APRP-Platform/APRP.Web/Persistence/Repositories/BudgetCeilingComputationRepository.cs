using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class BudgetCeilingComputationRepository : BaseRepository,IBudgetCeilingComputationRepository
    {
        private readonly ILogger _logger;
        public BudgetCeilingComputationRepository(AppDbContext context,
            ILogger<BudgetCeilingComputationRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                await _context.BudgetCeilingComputations.AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                return Ok(budgetCeilingComputation);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(budgetCeilingComputation);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public  async Task<IActionResult> FindByCodeAndFinancialYearIdAsync(string Code, long FinancialYearID)
        {
            try
            {
                var budgetCeilingComputation = await _context.BudgetCeilingComputations
                    .Where(x=>x.Code==Code && x.FinancialYearId== FinancialYearID)
                    .Include(a=>a.Authority)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(budgetCeilingComputation);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByBudgetVoteEntryAsync(BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                var budgetVoteListObject = await _context.BudgetCeilingComputations
                    .Where(c => c.Code == budgetCeilingComputation.Code ||
                    c.Name.ToLower() == budgetCeilingComputation.Name.ToLower())
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(budgetVoteListObject);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.FindByDisbursementCodeEntryAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var budgetCeilingComputation= await _context.BudgetCeilingComputations
                    .Include(a=>a.Authority)
                    .Include(f=>f.FinancialYear)
                    .Where(x=>x.ID==ID)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(budgetCeilingComputation);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var budgetCeilingComputationList = await _context.BudgetCeilingComputations
                    .Include(x=>x.Authority)
                    .ToListAsync().ConfigureAwait(false); 
                return Ok(budgetCeilingComputationList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public async Task<IActionResult> ListAsync(long FinancialYearId)
        {
            try
            {
                var budgetCeilingComputationList = await _context.BudgetCeilingComputations
                    .Where(x=>x.FinancialYearId==FinancialYearId)
                    .Include(x => x.Authority)
                    .ToListAsync().ConfigureAwait(false);
                return Ok(budgetCeilingComputationList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public async Task<IActionResult> ListAsync(string Code)
        {
            try
            {
                var budgetCeilingComputationList = await _context.BudgetCeilingComputations
                    .Where(x => x.Code == Code)
                    .Include(x => x.Authority)
                     .Include(b => b.FinancialYear)
                    .ToListAsync().ConfigureAwait(false);
                return Ok(budgetCeilingComputationList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                 _context.BudgetCeilingComputations.Remove(budgetCeilingComputation) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                _context.BudgetCeilingComputations.Update(budgetCeilingComputation);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(long ID, BudgetCeilingComputation budgetCeilingComputation)
        {            
            try
            {
                _context.Entry(budgetCeilingComputation).State = EntityState.Modified;
                return Ok(budgetCeilingComputation);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetachFirstEntryAsync(BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                _context.Entry(budgetCeilingComputation).State = EntityState.Detached;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
