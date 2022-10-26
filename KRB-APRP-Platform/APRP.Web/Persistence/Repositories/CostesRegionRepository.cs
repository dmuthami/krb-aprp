using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class CostesRegionRepository : BaseRepository,ICostesRegionRepository
    {
        private readonly ILogger _logger;
        public CostesRegionRepository(AppDbContext context,
            ILogger<CostesRegionRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(CostesRegion costesRegion)
        {
            try
            {
                await _context.CostesRegions.AddAsync(costesRegion).ConfigureAwait(false);
                return Ok(costesRegion);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"CostesRegionRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(costesRegion);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(int ID)
        {
            try
            {
                var costesRegion= await _context.CostesRegions.FindAsync(ID).ConfigureAwait(false);
                return Ok(costesRegion);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"CostesRegionRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var costesRegionList = await _context.CostesRegions.ToListAsync().ConfigureAwait(false); ;
                return Ok(costesRegionList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"CostesRegionRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(CostesRegion costesRegion)
        {
            try
            {
                 _context.CostesRegions.Remove(costesRegion) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"CostesRegionRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(CostesRegion costesRegion)
        {
            try
            {
                _context.CostesRegions.Update(costesRegion);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"CostesRegionRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(int ID, CostesRegion costesRegion)
        {            
            try
            {
                _context.Entry(costesRegion).State = EntityState.Modified;
                return Ok(costesRegion);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"CostesRegionRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
