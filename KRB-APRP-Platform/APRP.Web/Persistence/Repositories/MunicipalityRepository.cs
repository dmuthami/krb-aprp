using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class MunicipalityRepository : BaseRepository,IMunicipalityRepository
    {
        private readonly ILogger _logger;
        public MunicipalityRepository(AppDbContext context,
            ILogger<MunicipalityRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(Municipality municipality)
        {
            try
            {
                await _context.Municipalitys.AddAsync(municipality).ConfigureAwait(false);
                return Ok(municipality);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"MunicipalityRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(municipality);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(int ID)
        {
            try
            {
                var arics= await _context.Municipalitys.FindAsync(ID).ConfigureAwait(false);
                return Ok(arics);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"MunicipalityRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsYearList = await _context.Municipalitys.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsYearList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"MunicipalityRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(Municipality municipality)
        {
            try
            {
                 _context.Municipalitys.Remove(municipality) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"MunicipalityRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(Municipality municipality)
        {
            try
            {
                _context.Municipalitys.Update(municipality);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"MunicipalityRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(int ID, Municipality municipality)
        {            
            try
            {
                _context.Entry(municipality).State = EntityState.Modified;
                return Ok(municipality);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"MunicipalityRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
