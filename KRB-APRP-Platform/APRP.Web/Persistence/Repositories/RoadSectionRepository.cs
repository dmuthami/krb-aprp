using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadSectionRepository : BaseRepository, IRoadSectionRepository
    {
        public RoadSectionRepository(AppDbContext appDbContext) : base(appDbContext) { }

        public async Task AddAsync(RoadSection roadSection)
        {
            await _context.RoadSections.AddAsync(roadSection).ConfigureAwait(false);
        }

        public async Task DetachFirstEntryAsync(RoadSection roadSection)
        {
            _context.Entry(roadSection).State = EntityState.Detached;
        }

        public async Task<RoadSection> FindByDisbursementEntryAsync(RoadSection roadSection)
        {
            return await _context.RoadSections
            .Where(d => d.SectionID == roadSection.SectionID
            && d.RoadId == roadSection.RoadId)
            .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<RoadSection> FindByIdAsync(long ID)
        {
            var roadSection = await _context.RoadSections                
                .FindAsync(ID).ConfigureAwait(false);
            //Explicitly add Road 
            await _context.Entry(roadSection)
            .Reference(b => b.Road)
            .LoadAsync().ConfigureAwait(false);

            //Explicitly add Constituency 
            await _context.Entry(roadSection)
            .Reference(b => b.Constituency)
            .LoadAsync().ConfigureAwait(false);

            //Explicitly add surface Type
            await _context.Entry(roadSection)
            .Reference(s => s.SurfaceType)
            .LoadAsync().ConfigureAwait(false);

            return roadSection;

        }

        public async Task<RoadSection> FindBySectionIdAsync(string SectionID)
        {
            var roadSection = await _context.RoadSections
                .Include(r=>r.Road)
                .ThenInclude(a=>a.Authority)
                .Where(s => s.SectionID.ToLower() == SectionID.ToLower())
                .FirstOrDefaultAsync().ConfigureAwait(false);
            return roadSection;
        }

        public async Task<RoadSection> FindBySectionIdAsync(string SectionID, long AuthorityId)
        {
            var roadSection = await _context.RoadSections
             .Include(r => r.Road)
             .ThenInclude(a => a.Authority)
             .Where(s => s.SectionID.ToLower() == SectionID.ToLower() && s.Road.AuthorityId== AuthorityId)
             .FirstOrDefaultAsync().ConfigureAwait(false);
            return roadSection;
        }

        public async Task<IEnumerable<RoadSection>> GetRoadSectionsForAgencyAsync(Authority authority, string SurfaceType)
        {
            if (SurfaceType==null)
            {
                var roadSections = await _context.RoadSections
                    .Where(s => s.Road.AuthorityId == authority.ID)
                    .Include(R => R.Road)
                    .ToListAsync().ConfigureAwait(false);
                return roadSections;
            }
            else
            {
                return await _context.RoadSections
                    .Where(s => s.Road.AuthorityId == authority.ID && s.SurfaceType.Name == SurfaceType)
                    .Include(R => R.Road)
                    .ToListAsync().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<RoadSection>> ListAsync()
        {
            return await _context.RoadSections
                .Include(rwp=> rwp.RoadWorkSectionPlan)
                .Include(arics=>arics.Road)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadSection>> ListByRoadIdAsync(long roadID)
        {
            return await _context.RoadSections
                .Where(rs=> rs.Road.ID == roadID)
                .Include(s=> s.SurfaceType)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadSection>> ListUnPlannedSectionsByRoadIdAsync(long roadId,long financialYearId)
        {
            return await _context.RoadSections
                .Include(s => s.SurfaceType)
                .Include(w => w.RoadWorkSectionPlan).ThenInclude(y=>y.FinancialYear)
                .Where(rs => rs.Road.ID == roadId && rs.RoadWorkSectionPlan == null && rs.RoadWorkSectionPlan.FinancialYearId == financialYearId )
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IQueryable<RoadSection>> ListViewAsync()
        {

            IQueryable<RoadSection> x = null;
            await Task.Run(() =>
            {
                x = _context.RoadSections
                .Include(rwp => rwp.RoadWorkSectionPlan)
                .Include(arics => arics.Road);
            }).ConfigureAwait(false);
            return x;

        }

        public async Task<IQueryable<RoadSection>> ListViewAsync(Authority authority)
        {
            IQueryable<RoadSection> x = null;
            await Task.Run(() =>
            {
                x = _context.RoadSections
                .Where(a=>a.Road.AuthorityId==authority.ID)
                .Include(rwp => rwp.RoadWorkSectionPlan)
                .Include(arics => arics.Road);
            }).ConfigureAwait(false);
            return x;
        }

        public void Remove(RoadSection roadSection)
        {
            _context.RoadSections.Remove(roadSection);
        }

        public void Update(RoadSection roadSection)
        {
            _context.RoadSections.Update(roadSection);
        }

        public void Update(long ID, RoadSection roadSection)
        {
            _context.Entry(roadSection).State = EntityState.Modified;
        }
    }
}
