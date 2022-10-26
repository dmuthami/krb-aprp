using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class TrainingResponse : BaseResponse
    {
        public Training Training { get; set; }


        private TrainingResponse(bool success, string message, Training training) : base(success, message)
        {
            Training = training;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="Training">Saved Training.</param>
        /// <returns>Response.</returns>
        public TrainingResponse(Training training) : this(true, string.Empty, training)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public TrainingResponse(string message) : this(false, message, null)
        { }
    }
}
