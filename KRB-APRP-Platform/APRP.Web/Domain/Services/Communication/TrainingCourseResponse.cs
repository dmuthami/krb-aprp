using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class TrainingCourseResponse : BaseResponse
    {
        public TrainingCourse TrainingCourse { get; set; }


        private TrainingCourseResponse(bool success, string message, TrainingCourse trainingCourse) : base(success, message)
        {
            TrainingCourse = trainingCourse;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="TrainingCourse">Saved TrainingCourse.</param>
        /// <returns>Response.</returns>
        public TrainingCourseResponse(TrainingCourse trainingCourse) : this(true, string.Empty, trainingCourse)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public TrainingCourseResponse(string message) : this(false, message, null)
        { }
    }
}
