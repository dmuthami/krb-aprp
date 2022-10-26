using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class TrainingCourseListResponse : BaseResponse
    {
        public IEnumerable<TrainingCourse> TrainingCourse { get; set; }


        private TrainingCourseListResponse(bool success, string message, IEnumerable<TrainingCourse> trainingCourse) : base(success, message)
        {
            TrainingCourse =trainingCourse;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public TrainingCourseListResponse(IEnumerable<TrainingCourse> trainingCourse) : this(true, string.Empty, trainingCourse)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public TrainingCourseListResponse(string message) : this(false, message, null)
        { }
    }
}
