using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ITrainingCourseService
    {
        Task<TrainingCourseListResponse> ListAsync();
        Task<TrainingCourseResponse> AddAsync(TrainingCourse trainingCourse);
        Task<TrainingCourseResponse> FindByIdAsync(long ID);
        Task<TrainingCourseResponse> FindByCourseAsync(string Course);
        Task<TrainingCourseResponse> Update(TrainingCourse trainingCourse);
        Task<TrainingCourseResponse> Update(long ID, TrainingCourse trainingCourse);
        Task<TrainingCourseResponse> RemoveAsync(long ID);
    }
}
