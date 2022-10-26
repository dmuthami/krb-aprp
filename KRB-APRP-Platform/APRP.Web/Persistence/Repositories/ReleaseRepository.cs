using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ReleaseRepository : BaseRepository, IReleaseRepository
    {
        private readonly ILogger _logger;
        public ReleaseRepository(AppDbContext context, ILogger<ReleaseRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task AddAsync(Release release)
        {
            await _context.Releases.AddAsync(release).ConfigureAwait(false);
        }

        public async Task DetachFirstEntryAsync(Release release)
        {
            _context.Entry(release).State = EntityState.Detached;
        }

        public async Task<Release> FindByReleaseEntryAsync(Release release)
        {
            return await _context.Releases
            .Where(x => x.ChequeNo == release.ChequeNo &&
            x.DetailsOrPayee == release.DetailsOrPayee &&
            x.ReleaseAmount == release.ReleaseAmount)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        }
        public async Task<Disbursement> FindDisbursementByReleaseAsync(Release release)
        {
            DisbursementRelease disbusrementReleaseOfEinterest = null;
            var disbursements = await _context.Disbursements
             .Include(i => i.DisbursementReleases)
                .ToListAsync().ConfigureAwait(false);

            foreach (var disbursement in disbursements)
            {
                if (disbusrementReleaseOfEinterest == null)
                {
                    disbusrementReleaseOfEinterest = disbursement.DisbursementReleases
                       .Where(x => x.ReleaseId == release.ID)
                       .FirstOrDefault();
                }

            }
            await _context.Entry(disbusrementReleaseOfEinterest)
                .Reference(x => x.Disbursement).LoadAsync().ConfigureAwait(false);

            return disbusrementReleaseOfEinterest.Disbursement;

        }
        public async Task<Release> FindByIdAsync(long ID)
        {
            return await _context.Releases
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Release>> ListAsync()
        {
            return await _context.Releases
                            .ToListAsync().ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListDisbursementByReleaseAsync(Release release)
        {

            try
            {
                IEnumerable<Disbursement> disbursementsEnum = new List<Disbursement>();
                IList<Disbursement> disbursements = (IList<Disbursement>)disbursementsEnum;

                var disbursementReleases = await _context.DisbursementReleases
                 .Where(x => x.ReleaseId == release.ID)
                 .Include(i => i.Disbursement)
                 .ToListAsync().ConfigureAwait(false);

                foreach (var disbursementRelease in disbursementReleases)
                {

                    disbursements.Add(disbursementRelease.Disbursement);
                }

                return Ok(disbursements);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }

        }

        public void Remove(Release release)
        {
            _context.Releases.Remove(release);
        }

        public void Update(Release release)
        {
            _context.Releases.Update(release);
        }

        public void Update(long ID, Release release)
        {
            _context.Entry(release).State = EntityState.Modified;
        }

        public async Task<IEnumerable<Release>> ListAsync(long FinancialYearId)
        {
            IEnumerable<Release> releasesEnum = new List<Release>();
            IList<Release> releases = (IList<Release>)releasesEnum;

            var disbursements = await _context.Disbursements
                .Where(w => w.FinancialYearId == FinancialYearId)
                .Include(i => i.DisbursementReleases)
                   .ThenInclude(v => v.Release)
                   .ToListAsync().ConfigureAwait(false);

            foreach (var disbursment in disbursements)
            {
                foreach (var disbursementRelease in disbursment.DisbursementReleases)
                {
                    var releasefind = releases.Where(m => m.ID == disbursementRelease.ReleaseId).FirstOrDefault();
                    if (releasefind == null)
                    {
                        releases.Add(disbursementRelease.Release);
                    }
                }
            }
            return releases;
        }

        public async Task<IEnumerable<Release>> ListAsync(long FinancialYearId, string Code)
        {
            IEnumerable<Release> releasesEnum = new List<Release>();
            IList<Release> releases = (IList<Release>)releasesEnum;

            var disbursements = await _context.Disbursements
                .Where(w => w.FinancialYearId == FinancialYearId
                && w.BudgetCeilingComputation.Code == Code)
                .Include(y => y.BudgetCeilingComputation)
                .Include(i => i.DisbursementReleases)
                   .ThenInclude(v => v.Release)
                 .ToListAsync().ConfigureAwait(false);

            foreach (var disbursment in disbursements)
            {
                foreach (var DisbursementRelease in disbursment.DisbursementReleases)
                {
                    //check if release has already been added
                    if (releases.Any())
                    {
                        var existingRelease = releases.Where(w => w.ID == DisbursementRelease.Release.ID)
                            .FirstOrDefault();
                        if (existingRelease == null)
                        {
                            //Add new release
                            releases.Add(DisbursementRelease.Release);
                        }
                    }
                    else
                    {
                        releases.Add(DisbursementRelease.Release);
                    }
                }
            }

            return releases;
        }

        public async Task<IEnumerable<Release>> ListAsync2(long FinancialYearId, string Code, DateTime startDate, DateTime endDate)
        {
            IEnumerable<Release> releasesEnum = new List<Release>();
            IList<Release> releases = (IList<Release>)releasesEnum;

            var disbursements = await _context.Disbursements
                .Where(w =>
                //w.FinancialYearId == FinancialYearId && 
                w.BudgetCeilingComputation.Code == Code)
                .Include(y => y.BudgetCeilingComputation)
                .Include(i => i.DisbursementReleases)
                   .ThenInclude(v => v.Release)
                 .ToListAsync().ConfigureAwait(false);

            foreach (var disbursment in disbursements)
            {
                foreach (var DisbursementRelease in disbursment.DisbursementReleases)
                {
                    if (releases.Any())
                    {

                        //check if release has already been added
                        var existingRelease = releases.Where(w => w.ID == DisbursementRelease.Release.ID)
                            .FirstOrDefault();
                        if (existingRelease == null)
                        {
                            //Add new release if between start and end date included
                            var myRelease = DisbursementRelease.Release;
                            if (myRelease.ReleaseDate >= startDate && myRelease.ReleaseDate <= endDate)
                            {
                                releases.Add(DisbursementRelease.Release);
                            }
                        }
                    }
                    else
                    {
                        //Add new release if between start and end date included
                        var myRelease = DisbursementRelease.Release;
                        if (myRelease.ReleaseDate >= startDate && myRelease.ReleaseDate <= endDate)
                        {
                            releases.Add(DisbursementRelease.Release);
                        }
                    }
                }
            }

            return releases;
        }
    }
}
