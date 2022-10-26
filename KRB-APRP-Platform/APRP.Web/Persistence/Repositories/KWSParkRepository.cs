using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class KWSParkRepository : BaseRepository,IKWSParkRepository
    {
        private readonly ILogger _logger;
        public KWSParkRepository(AppDbContext context,
            ILogger<KWSParkRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(KWSPark kWSPark)
        {
            try
            {
                await _context.KWSParks.AddAsync(kWSPark).ConfigureAwait(false);
                return Ok(kWSPark);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"KWSParkRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(kWSPark);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(int ID)
        {
            try
            {
                var arics= await _context.KWSParks.FindAsync(ID).ConfigureAwait(false);
                return Ok(arics);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"KWSParkRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsYearList = await _context.KWSParks.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsYearList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"KWSParkRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(KWSPark kWSPark)
        {
            try
            {
                 _context.KWSParks.Remove(kWSPark) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"KWSParkRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(KWSPark kWSPark)
        {
            try
            {
                _context.KWSParks.Update(kWSPark);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"KWSParkRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(int ID, KWSPark kWSPark)
        {            
            try
            {
                _context.Entry(kWSPark).State = EntityState.Modified;
                return Ok(kWSPark);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"KWSParkRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
