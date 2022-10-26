using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ARICSRepository : BaseRepository, IARICSRepository
    {
        private readonly ILogger _logger;
        public ARICSRepository(AppDbContext context, ILogger<ARICSRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(ARICS aRICS)
        {
            await _context.ARICSS.AddAsync(aRICS).ConfigureAwait(false);
        }

        public async Task<ARICS> CheckARICSForSheet(long SheetId)
        {
            var aRICS = await _context.ARICSS
            .Where(s => s.RoadSheetId == SheetId)
            .FirstOrDefaultAsync().ConfigureAwait(false);
            return aRICS;
        }

        public async Task<ARICS> FindByIdAsync(long ID)
        {
            return await _context.ARICSS
                .FindAsync(ID).
                ConfigureAwait(false);
        }

        public async Task<ARICS> FindByRoadSheetAndChainageAsync(long RoadSheetID, int Chainage)
        {
            var aRICS = await _context.ARICSS
            .Where(s => s.RoadSheetId == RoadSheetID && s.Chainage == Chainage)
            .FirstOrDefaultAsync().ConfigureAwait(false);
            return aRICS;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GetARICEDRoadSectionByAuthorityAndYear(long AuthorityId, int ARICSYear)
        {
            try
            {
                var roadSheetList = await _context.RoadSheets
                    .Include(i=>i.RoadSection)
                        .ThenInclude(i=>i.Road)
                    .Where(i=>i.RoadSection.Road.AuthorityId==AuthorityId && 
                    i.Year== ARICSYear)
                    .ToListAsync()
                    .ConfigureAwait(false); ;
                return Ok(roadSheetList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSRepository.GetARICEDRoadSectionByAuthorityAndYear Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public async Task<IEnumerable<ARICS>> GetARICS(int? Year)
        {
            int _Year = DateTime.UtcNow.Year;
            bool result = int.TryParse(Year.ToString(), out _Year);
            if (result == false)
            {
                _Year = DateTime.UtcNow.Year;
            }

            //Get arics for the year supplied
            var aRICS = await _context.ARICSS
            .Where(s => s.RoadSheet.Year == _Year)
            .Include(r => r.RoadSheet)
                .ThenInclude(rds => rds.RoadSection)
                    .ThenInclude(rd => rd.Road)
                        .ThenInclude(rc => rc.RoadConditions)
            .ToListAsync().ConfigureAwait(false);

            return aRICS;
        }

        public async Task<IQueryable<ARICS>> GetARICS2(Authority authority,int? Year)
        {
            int _Year = DateTime.UtcNow.Year;
            bool result = int.TryParse(Year.ToString(), out _Year);
            if (result == false)
            {
                _Year = DateTime.UtcNow.Year;
            }

            IQueryable<ARICS> x = null;
            await Task.Run(() =>
            {
                x = _context.ARICSS
                .Where(rs => rs.RoadSheet.RoadSection.Road.AuthorityId == authority.ID &&
                    rs.RoadSheet.Year == _Year)
                .Include(rsheet => rsheet.RoadSheet)
                .ThenInclude(rs => rs.RoadSection)
                .ThenInclude(rd=>rd.Road);
            }).ConfigureAwait(false);
            return x;
        }
        public async Task<IEnumerable<ARICS>> GetARICSByRoadSection(long RoadSectionId)
        {
            IEnumerable<ARICS> aRICsTotal = null;
            var tsk = await _context.RoadSheets
                 .Where(w => w.RoadSectionId == RoadSectionId).ToListAsync().ConfigureAwait(false);
            foreach (var roadSheet in tsk)
            {
                var aRICS = await _context.ARICSS
                .Where(s => s.RoadSheetId == roadSheet.ID).ToListAsync().ConfigureAwait(false);
                if (aRICsTotal == null)
                {
                    aRICsTotal = aRICS;
                }
                else
                {
                    aRICsTotal.Concat(aRICS);
                }
            }
            return aRICsTotal;
        }

        public async Task<IList<ARICS>> GetARICSBySheetNo(long ID)
        {
            var aRICS = await _context.ARICSS
            .Where(s => s.RoadSheetId == ID)
            .Include(x => x.RoadSheet)
            .OrderBy(o => o.ID)
            //.ThenInclude(y=>y.RoadSection)
            //    .ThenInclude(z=>z.Constituency)
            //        .ThenInclude(a=>a.County)
            .ToListAsync().ConfigureAwait(false);

            return aRICS;
        }

        /// <summary>
        /// Similar to FindByIdAsync but explicitly loads Survey 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<ARICS> GetARICSDetails(long ID)
        {
            var aRICS = await _context.ARICSS
                .Where(w => w.ID == ID)
                .Include(x => x.ShoulderInterventionPaved)
                .Include(x => x.ShoulderSurfaceTypePaved)
                .Include(x => x.SurfaceTypeUnPaved)
                .Include(x => x.GravelRequired)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            return aRICS;
        }

        public async Task<IEnumerable<ARICS>> GetARICSForRoad(Road road, int? Year)
        {
            int _Year = DateTime.UtcNow.Year;
            bool result = int.TryParse(Year.ToString(), out _Year);
            if (result == false)
            {
                _Year = DateTime.UtcNow.Year;
            }

            var aRICS = await _context.ARICSS
            .Where(s => s.RoadSheet.RoadSection.Road.RoadNumber == road.RoadNumber && s.RoadSheet.Year == _Year)
            //.Include(s => s.RoadSection)
            .ToListAsync().ConfigureAwait(false);
            return aRICS;
        }

        public async Task<IEnumerable<ARICS>> GetARICSForRoad(Road road, double StartChainage, double EndChainage, int? Year)
        {
            int _startchainage = (int)Math.Ceiling(StartChainage);
            int _endchainage = (int)Math.Ceiling(EndChainage);

            int _Year = DateTime.UtcNow.Year;
            bool result = int.TryParse(Year.ToString(), out _Year);
            if (result == false)
            {
                _Year = DateTime.UtcNow.Year;
            }

            var aRICS = await _context.ARICSS
            .Where(s =>
                (s.RoadSheet.RoadSection.Road.RoadNumber == road.RoadNumber && s.RoadSheet.Year == Year)
                &&
                (s.Chainage > _startchainage && s.Chainage < _endchainage)
                )
            .ToListAsync().ConfigureAwait(false);
            return aRICS;
        }

        public async Task<IList<ARICS>> GetARICSForSheetNo(long SheetID)
        {
            var aRICS = await _context.ARICSS
            .Where(s => s.RoadSheetId == SheetID)
            //.Include(s => s.RoadSection)
            .ToListAsync().ConfigureAwait(false);
            return aRICS;
        }

        public async Task<IEnumerable<ARICS>> ListAsync()
        {
            return await _context.ARICSS.ToListAsync().ConfigureAwait(false);
        }

        public void Remove(ARICS aRICS)
        {
            _context.ARICSS.Remove(aRICS);
        }

        public void Update(ARICS aRICS)
        {
            _context.ARICSS.Update(aRICS);
        }

        public void Update(long ID, ARICS aRICS)
        {
            _context.Entry(aRICS).State = EntityState.Modified;
        }
    }
}
