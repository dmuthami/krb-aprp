using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ITrainingCourseRepository
    {
        Task<IEnumerable<TrainingCourse>> ListAsync();

        Task AddAsync(TrainingCourse trainingCourses);
        Task<TrainingCourse> FindByIdAsync(long ID);

        Task<TrainingCourse> FindByCourseAsync(string Course);

        void Update(TrainingCourse trainingCourses);
        void Update(long ID, TrainingCourse trainingCourses);

        void Remove(TrainingCourse trainingCourses);

    }
}
