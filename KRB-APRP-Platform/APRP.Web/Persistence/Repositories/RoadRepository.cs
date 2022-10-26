using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadRepository : BaseRepository, IRoadRepository
    {
        private readonly ILogger _logger;
        public RoadRepository(AppDbContext context,
            ILogger<RoadRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(Road road)
        {
            await _context.Roads.AddAsync(road).ConfigureAwait(false);
        }

        public async Task<Road> FindByIdAsync(long ID)
        {
            var road = await _context.Roads
                .Include(s => s.RoadSections)
                .ThenInclude(t => t.SurfaceType)
                .Include(p => p.RoadWorkSectionPlans)
                .Include(a => a.Authority)
                .Include(r => r.RoadConditions)
                //.ThenInclude(rp=>rp.RoadPrioritization)
                .SingleOrDefaultAsync(i => i.ID == ID).ConfigureAwait(false);
            return road;
        }

        public async Task<IEnumerable<Road>> RoadNumberAjaxListAsync(string RoadNumber)
        {
            return await _context.Roads
                .Where(s => s.RoadNumber.ToLower().Contains(RoadNumber.ToLower()))
                .ToListAsync().ConfigureAwait(true);
        }

        public async Task<IEnumerable<Road>> ListAsync()
        {
            return await _context.Roads
                .Include(rs => rs.RoadSections)
                .Include(p => p.RoadWorkSectionPlans)
                .OrderBy(o => o.RoadNumber)
                .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(Road road)
        {
            _context.Roads.Remove(road);
        }

        public void Update(Road road)
        {
            _context.Roads.Update(road);
        }

        public async Task<Road> FindByRoadNumberAsync(string RoadNumber)
        {
            return await _context.Roads
                .Where(s => s.RoadNumber == RoadNumber)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IActionResult> ListAsync(Authority authority)
        {
            try
            {
                var roads = await _context.Roads
                 .Where(w => w.AuthorityId == authority.ID)
                 .Include(rs => rs.RoadSections)
                 .Include(p => p.RoadWorkSectionPlans)
                 .OrderBy(o => o.RoadNumber)
                 .ToListAsync().ConfigureAwait(false);
                return Ok(roads);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(Ex);
            }
        }

        public async Task<IEnumerable<Road>> GetRoadWithSectionsAsync(Authority authority)
        {
            if (authority == null)
            {
                return await _context.Roads
                .Include(rs => rs.RoadSections)
                .OrderBy(o => o.RoadNumber)
                .ToListAsync().ConfigureAwait(false);
            }
            else
            {
                return await _context.Roads
                .Include(rs => rs.RoadSections)
                .Where(w => w.AuthorityId == authority.ID)
                .OrderBy(o => o.RoadNumber)
                .ToListAsync().ConfigureAwait(false);
            }
        }

        public async Task<IQueryable<Road>> ListViewAsync()
        {
            IQueryable<Road> x = null; ;
            await Task.Run(() =>
            {
                x = _context.Roads
                .Include(rs => rs.RoadSections)
                .Include(p => p.RoadWorkSectionPlans)
                .OrderBy(o => o.RoadNumber);
            }).ConfigureAwait(false);
            return x;
        }
        public async Task<IQueryable<Road>> ListViewAsync(Authority authority)
        {
            IQueryable<Road> x = null;
            await Task.Run(() =>
            {
                x = _context.Roads
                .Where(I => I.AuthorityId == authority.ID)
                .Include(rs => rs.RoadSections)
                .Include(p => p.RoadWorkSectionPlans)
                .OrderBy(o => o.RoadNumber);
            }).ConfigureAwait(false);
            return x;
        }

        public async Task<IQueryable<Road>> ListViewWithAricsAsync(Authority authority, int? Year)
        {
            IQueryable<Road> x = null;
            if (Year == null)
            {
                await Task.Run(() =>
                {
                    x = _context.Roads
                    .Where(I => I.AuthorityId == authority.ID)
                    .Include(rs => rs.RoadSections)
                    .Include(r => r.RoadConditions)
                    .OrderBy(o => o.RoadNumber);
                }).ConfigureAwait(false);
            }
            else
            {
                int _Year = DateTime.UtcNow.Year;
                bool result = int.TryParse(Year.ToString(), out _Year);
                if (result == false)
                {
                    _Year = DateTime.UtcNow.Year;
                }
                await Task.Run(() =>
                {
                    x = _context.Roads
                    .Where(I => I.AuthorityId == authority.ID)
                    .Include(rs => rs.RoadSections)
                    .Include(r => r.RoadConditions)
                    //.Where(x => x.RoadConditions.Any(a => a.Year == _Year))
                    .OrderBy(o => o.RoadNumber);
                }).ConfigureAwait(false);
            }

            return x;
        }

        public async Task<IQueryable<Road>> ListViewWithAricsAsync(int? Year)
        {
            IQueryable<Road> x = null;
            if (Year == null)
            {
                await Task.Run(() =>
                {
                    x = _context.Roads
                    .Include(rs => rs.RoadSections)
                    .Include(r => r.RoadConditions)
                    .OrderBy(o => o.RoadNumber);
                }).ConfigureAwait(false);
                return x;
            }
            else
            {
                int _Year = DateTime.UtcNow.Year;
                bool result = int.TryParse(Year.ToString(), out _Year);
                if (result == false)
                {
                    _Year = DateTime.UtcNow.Year;
                }
                await Task.Run(() =>
                {
                    x = _context.Roads
                    .Include(rs => rs.RoadSections)
                    .Include(r => r.RoadConditions)
                    //.Where(x => x.RoadConditions.Any(a => a.Year == _Year))
                    .OrderBy(o => o.RoadNumber);
                }).ConfigureAwait(false);
                return x;
            }

        }

        public async Task<Road> FindByRoadNumberAsync(long AuthorityId, string RoadNumber)
        {
            return await _context.Roads
                .Where(s => s.RoadNumber == RoadNumber && s.AuthorityId == AuthorityId)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public void Update(long ID, Road road)
        {
            _context.Entry(road).State = EntityState.Modified;
        }

        public async Task<Road> FindByIdAsync(long ID, int ARICSYear)
        {
            var road = await _context.Roads
                //.Where(w=>w.RoadConditions.)
                .Include(s => s.RoadSections)
                .ThenInclude(t => t.SurfaceType)
                .Include(p => p.RoadWorkSectionPlans)
                .Include(a => a.Authority)
                //.ThenInclude(rp=>rp.RoadPrioritization)
                .SingleOrDefaultAsync(i => i.ID == ID).ConfigureAwait(false);
            return road;
        }

        public async Task DetachFirstEntryAsync(Road road)
        {
            _context.Entry(road).State = EntityState.Detached;
        }

        public async Task<Road> FindByDisbursementEntryAsync(Road road)
        {
            return await _context.Roads
            .Where(d => d.AuthorityId == road.AuthorityId
            && d.RoadNumber == road.RoadNumber)
            .FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
