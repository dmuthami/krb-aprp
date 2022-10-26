using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadConditionRepository : BaseRepository, IRoadConditionRepository
    {
        public RoadConditionRepository(AppDbContext context) : base(context)
        {

        }
        public async Task AddAsync(RoadCondition roadCondtion)
        {
            await _context.RoadConditions.AddAsync(roadCondtion).ConfigureAwait(false);
        }

        public async Task DetachFirstEntryAsync(RoadCondition roadCondtion)
        {
            _context.Entry(roadCondtion).State = EntityState.Detached;
        }

        public async Task<RoadCondition> FindByIdAsync(long ID)
        {
            return await _context.RoadConditions.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<RoadCondition> FindByPriorityRateAsync(long AuthorityID, int? Year, long PriorityRate)
        {
            int _Year;

            bool result = int.TryParse(Year.ToString(), out _Year);
            if (result == false)
            {
                _Year = DateTime.UtcNow.Year;
            }
            var roadCondition = await _context.RoadConditions
                .Include(r => r.Road)
                .Where(s => s.Road.AuthorityId == AuthorityID && s.Year == _Year && s.PriorityRate == PriorityRate)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return roadCondition;
        }

        public async Task<RoadCondition> FindByRoadIdAsync(long RoadID, int? Year)
        {
            int _Year = DateTime.UtcNow.Year;
            bool result = int.TryParse(Year.ToString(), out _Year);
            if (result == false)
            {
                _Year = DateTime.UtcNow.Year;
            }
            var roadcondtion = await _context.RoadConditions
                .Where(s => s.RoadId == RoadID && s.Year == _Year)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return roadcondtion;
        }

        public async Task<RoadCondition> GetRoadConditionByYear(Road road, int? Year)
        {
            int _Year = DateTime.UtcNow.Year;
            bool result = int.TryParse(Year.ToString(), out _Year);
            if (result == false)
            {
                _Year = DateTime.UtcNow.Year;
            }

            var roadConditions = await _context.RoadConditions
            .Where(s => s.RoadId == road.ID && s.Year == _Year)
            .Include(r => r.Road)
                .ThenInclude(rs => rs.RoadSections)
            .FirstOrDefaultAsync().ConfigureAwait(false);
            return roadConditions;
        }

        public async Task<IEnumerable<RoadCondition>> ListAsync()
        {
            return await _context.RoadConditions
                .Include(r => r.Road)
                //.Include(rp => rp.RoadPrioritization)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadCondition>> ListAsync(int? Year)
        {
            int _Year;
            if (Year != null)
            {
                bool result = int.TryParse(Year.ToString(), out _Year);
            }
            else
            {
                _Year = DateTime.UtcNow.Year;
            }

            return await _context.RoadConditions
                .Where(y => y.Year == _Year)
                .Include(r => r.Road)
                    .ThenInclude(r => r.RoadSections)
                .Include(a => a.Road)
                    .ThenInclude(a => a.Authority)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadCondition>> ListAsync(Authority authority, int? Year)
        {
            int _Year;
            if (Year != null)
            {
                bool result = int.TryParse(Year.ToString(), out _Year);
            }
            else
            {
                _Year = DateTime.UtcNow.Year;
            }

            return await _context.RoadConditions
                .Where(y => y.Year == _Year && y.Road.AuthorityId == authority.ID)
                .Include(r => r.Road)
                    .ThenInclude(r => r.RoadSections)
                .Include(a=>a.Road)
                    .ThenInclude(a=>a.Authority)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public void Remove(RoadCondition roadCondtion)
        {
            _context.RoadConditions.Remove(roadCondtion);
        }
        public void Update(long ID, RoadCondition roadCondtion)
        {
            _context.Entry(roadCondtion).State = EntityState.Modified;
        }
    }
}
