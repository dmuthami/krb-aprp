using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadSheetRepository : BaseRepository, IRoadSheetRepository
    {
        private readonly ILogger _logger;
        public RoadSheetRepository(AppDbContext appDbContext, ILogger<RoadSheetRepository> logger) : base(appDbContext)
        {
            _logger = logger;
        }

        public async Task AddAsync(RoadSheet roadSheet)
        {
            await _context.RoadSheets.AddAsync(roadSheet);
        }

        public async Task<RoadSheet> FindByIdAsync(long ID)
        {
            var roadSheet = await _context.RoadSheets.FindAsync(ID);

            //Explicitly load roadsection
            await _context.Entry(roadSheet)
            .Reference(b => b.RoadSection)
            .LoadAsync().ConfigureAwait(false);

            //Explicitly load terrain Type
            await _context.Entry(roadSheet)
            .Reference(t => t.TerrainType)
            .LoadAsync().ConfigureAwait(false);

            return roadSheet;
        }

        public async Task<IEnumerable<RoadSheet>> ListByRoadSectionIdAsync(long RoadSectionID, int? Year)
        {
            if (Year == null)
            {
                var roadSheets = await _context.RoadSheets
                .Where(s => s.RoadSectionId == RoadSectionID)
                .Include(t => t.TerrainType)
                .Include(s => s.RoadSection)
                .ThenInclude(r => r.Road)
                .OrderBy(o => o.SheetNo)
                .ToListAsync().ConfigureAwait(false);
                return roadSheets;
            }
            else
            {
                int _Year = DateTime.UtcNow.Year;
                bool result = int.TryParse(Year.ToString(), out _Year);
                if (result == false)
                {
                    _Year = DateTime.UtcNow.Year;
                }
                var roadSheets = await _context.RoadSheets
                .Where(s => s.RoadSectionId == RoadSectionID && s.Year == _Year)
                .Include(t => t.TerrainType)
                .Include(s => s.RoadSection)
                .ThenInclude(r => r.Road)
                .OrderBy(o => o.SheetNo)
                .ToListAsync().ConfigureAwait(false);
                return roadSheets;
            }
        }

        public async Task<IEnumerable<RoadSheet>> ListAsync()
        {
            return await _context.RoadSheets.ToListAsync().ConfigureAwait(false);
        }


        public void Remove(RoadSheet roadSheet)
        {
            _context.RoadSheets.Remove(roadSheet);
        }

        public void Update(RoadSheet roadSheet)
        {
            _context.RoadSheets.Update(roadSheet);
        }

        public async Task<IEnumerable<RoadSheet>> DisplayRoadsheetsAsync(long RoadSectionID, int Year)
        {
            try
            {
                return await _context.RoadSheets
                    .Where(s => s.RoadSectionId == RoadSectionID && s.Year == Year)
                    .Include(s => s.RoadSection)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"RoadSheetRepository.DisplayRoadsheetsAsync Error : " +
                    $"{Ex.Message}");
                return null; ;
            }
        }

        public async Task<RoadSheet> CheckRoadSheetsForYear(RoadSheetVM _RoadSheetVM)
        {
            var roadSheet = await _context.RoadSheets
            .Where(s => s.RoadSectionId == _RoadSheetVM.RoadSectionID
            && s.Year == _RoadSheetVM.Year)
            .FirstOrDefaultAsync().ConfigureAwait(false);

            return roadSheet;
        }

        public void Update(long ID, RoadSheet roadSheet)
        {
            _context.Entry(roadSheet).State = EntityState.Modified;
        }

    }
}
