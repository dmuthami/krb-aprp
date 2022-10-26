using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class TrainingRepository : BaseRepository, ITrainingRepository
    {
        public TrainingRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task AddAsync(Training training)
        {
            await _context.Trainings.AddAsync(training).ConfigureAwait(false);
        }


        public async Task<Training> FindByIdAsync(long ID)
        {
            return await _context.Trainings
                .Include(a => a.Authority)
                .Include(t => t.TrainingCourse)
                .Include(q => q.QuarterCodeUnit)
                    .ThenInclude(f => f.FinancialYear)
                 .Include(q => q.QuarterCodeUnit)
                    .ThenInclude(f => f.QuarterCodeList)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<Training> FindByQuarterCodeUnitIdAndAuthorityIdAsync(long QuarterCodeUnitId, long AuthorityId)
        {
            return await _context.Trainings
            .Include(a => a.Authority)
            .Include(t => t.TrainingCourse)
            .Include(q => q.QuarterCodeUnit)
                .ThenInclude(f => f.FinancialYear)
            .Include(q => q.QuarterCodeUnit)
                .ThenInclude(f => f.QuarterCodeList)
            .FirstOrDefaultAsync(m => m.QuarterCodeUnitId == QuarterCodeUnitId && m.AuthorityId == AuthorityId).ConfigureAwait(false);
        }

        public async Task<Training> FindByQuarterCodeUnitIdAndFinancialIdAsync(long QuarterCodeUnitId, long FinancialYearId)
        {
            return await _context.Trainings
            .Include(a => a.Authority)
            .Include(t => t.TrainingCourse)
            .Include(q => q.QuarterCodeUnit)
                .ThenInclude(f=>f.FinancialYear)
            .Include(q => q.QuarterCodeUnit)
                .ThenInclude(f => f.QuarterCodeList)
            .FirstOrDefaultAsync(m => m.QuarterCodeUnitId == QuarterCodeUnitId && m.QuarterCodeUnit.FinancialYear.ID== FinancialYearId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Training>> GetCourceTrainingsThisFinancialYearAsync(int TrainingCourseID, long FinancialYearId)
        {
            return await _context.Trainings
                .Where(s => s.TrainingCourse.ID == TrainingCourseID && s.QuarterCodeUnit.FinancialYearId == FinancialYearId)
                .Include(t => t.TrainingCourse)
                .Include(a => a.Authority)
                .Include(q => q.QuarterCodeUnit)
                    .ThenInclude(f => f.FinancialYear)
                .Include(q => q.QuarterCodeUnit)
                    .ThenInclude(f => f.QuarterCodeList)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Training>> GetCourceTrainingsThisQuarterAsync(int TrainingCourseID, int quarter)
        {
            return await _context.Trainings
                .Where(s=>s.TrainingCourse.ID== TrainingCourseID && s.QuarterCodeUnit.QuarterCodeList.ReferenceID==quarter)
                .Include(t => t.TrainingCourse)
                .Include(a => a.Authority)
                .Include(q => q.QuarterCodeUnit)
                    .ThenInclude(f => f.FinancialYear)
                .Include(q => q.QuarterCodeUnit)
                    .ThenInclude(f => f.QuarterCodeList)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<TrainingCourse>> GetDistinctCoursesByFinancialYearAndAuthorityAsync(long AuthorityId, long FinancialYearId)
        {
            IEnumerable<TrainingCourse> trainingCourseEnum = new List<TrainingCourse>();
            IList<TrainingCourse> trainingCourses = (IList<TrainingCourse>)trainingCourseEnum;


            //get training courses for authority and in specific financial year
            var trainings = await _context.Trainings
                    .Where(s => s.AuthorityId == AuthorityId && s.QuarterCodeUnit.FinancialYearId == FinancialYearId)
                    .Include(t => t.TrainingCourse)
                    .Include(a => a.Authority)
                    .Include(q => q.QuarterCodeUnit)
                        .ThenInclude(f => f.FinancialYear)
                    .Include(q => q.QuarterCodeUnit)
                        .ThenInclude(f => f.QuarterCodeList)
                    .ToListAsync().ConfigureAwait(false);

            foreach (var training in trainings)
            {
                //get course
                var myTrainingCourse = training.TrainingCourse;
                if (trainingCourses.Any())
                {
                    var existingTrainingCourse = trainingCourses.Where(w => w.ID == myTrainingCourse.ID)
                    .FirstOrDefault();
                    if (existingTrainingCourse == null)
                    {
                        trainingCourses.Add(myTrainingCourse);
                    }
                }
                else
                {
                    trainingCourses.Add(myTrainingCourse);
                }
            }

            return trainingCourses;
        }

        public async Task<IEnumerable<Training>> ListAsync()
        {
            return await _context.Trainings
                .Include(a => a.Authority)
                .Include(t => t.TrainingCourse)
                .Include(q => q.QuarterCodeUnit)
                    .ThenInclude(f => f.FinancialYear)
                .Include(q => q.QuarterCodeUnit)
                    .ThenInclude(f => f.QuarterCodeList)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Training>> ListAsync(long AuthorityId)
        {
            return await _context.Trainings
            .Where(w => w.AuthorityId == AuthorityId)
            .Include(a => a.Authority)
            .Include(t => t.TrainingCourse)
            .Include(q => q.QuarterCodeUnit)
                .ThenInclude(f => f.FinancialYear)
            .Include(q => q.QuarterCodeUnit)
                .ThenInclude(f => f.QuarterCodeList)
            .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Training>> ListAsync(long AuthorityId, long FinancialYearId)
        {
            return await _context.Trainings
             .Where(w => w.QuarterCodeUnit.FinancialYearId == FinancialYearId
             && w.AuthorityId == AuthorityId)
             .Include(a => a.Authority)
             .Include(t => t.TrainingCourse)
             .Include(q => q.QuarterCodeUnit)
                 .ThenInclude(f => f.FinancialYear)
             .Include(q => q.QuarterCodeUnit)
                 .ThenInclude(f => f.QuarterCodeList)
             .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Training>> ListByFinancialYearAsync(long FinancialYearId)
        {
            return await _context.Trainings
             .Where(w => w.QuarterCodeUnit.FinancialYearId == FinancialYearId)
             .Include(a => a.Authority)
             .Include(t => t.TrainingCourse)
             .Include(q => q.QuarterCodeUnit)
                 .ThenInclude(f => f.FinancialYear)
             .Include(q => q.QuarterCodeUnit)
                 .ThenInclude(f => f.QuarterCodeList)
             .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Training>> ListByTrainingCourseInFinancialYearAsync(long TrainingCourseId, long FinancialYearId)
        {
            return await _context.Trainings
                    .Where(w => w.QuarterCodeUnit.FinancialYearId == FinancialYearId && w.TrainingCourseId== TrainingCourseId)
                    .Include(a => a.Authority)
                    .Include(t => t.TrainingCourse)
                    .Include(q => q.QuarterCodeUnit)
                        .ThenInclude(f => f.FinancialYear)
                    .Include(q => q.QuarterCodeUnit)
                        .ThenInclude(f => f.QuarterCodeList)
                    .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Training>> ListDistinctCoursesByFinancialYearAsync(long FinancialYearId)
        {
            //Select distinct courses only
            return await _context.Trainings
             .Where(w => w.QuarterCodeUnit.FinancialYearId == FinancialYearId)
             .GroupBy(c=>c.TrainingCourse.Course).Select(g=>g.FirstOrDefault())
             .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(Training training)
        {
            _context.Trainings.Remove(training);
        }

        public void Update(Training training)
        {
            _context.Trainings.Update(training);
        }

        public void Update(long ID, Training training)
        {
            _context.Entry(training).State = EntityState.Modified;
        }
    }
}
