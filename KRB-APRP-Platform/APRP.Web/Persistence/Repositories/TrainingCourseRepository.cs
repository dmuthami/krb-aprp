using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class TrainingCourseRepository : BaseRepository, ITrainingCourseRepository
    {
        public TrainingCourseRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task AddAsync(TrainingCourse trainingCourse)
        {
            await _context.TrainingCourses.AddAsync(trainingCourse).ConfigureAwait(false);
        }

        public async Task<TrainingCourse> FindByIdAsync(long ID)
        {
            return await _context.TrainingCourses
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<TrainingCourse> FindByCourseAsync(string Course)
        {
            return await _context.TrainingCourses
                .FirstOrDefaultAsync(m => m.Course.ToLower() == Course.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TrainingCourse>> ListAsync()
        {
            return await _context.TrainingCourses
                  .ToListAsync().ConfigureAwait(false);
        }


        public void Remove(TrainingCourse trainingCourse)
        {
            _context.TrainingCourses.Remove(trainingCourse);
        }

        public void Update(TrainingCourse trainingCourse)
        {
            _context.TrainingCourses.Update(trainingCourse);
        }

        public void Update(long ID, TrainingCourse trainingCourse)
        {
            _context.Entry(trainingCourse).State = EntityState.Modified;
        }
    }
}
