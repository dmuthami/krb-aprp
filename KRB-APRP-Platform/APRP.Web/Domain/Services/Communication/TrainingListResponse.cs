using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class TrainingListResponse : BaseResponse
    {
        public IEnumerable<Training> Training { get; set; }


        private TrainingListResponse(bool success, string message, IEnumerable<Training> training) : base(success, message)
        {
            Training =training;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public TrainingListResponse(IEnumerable<Training> training) : this(true, string.Empty, training)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public TrainingListResponse(string message) : this(false, message, null)
        { }
    }
}
