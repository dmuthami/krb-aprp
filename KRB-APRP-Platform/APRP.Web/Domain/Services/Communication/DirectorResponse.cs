using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class DirectorResponse : BaseResponse
    {
        public Director Director { get; set; }
        private DirectorResponse(bool success, string message, Director director) : base(success, message)
        {
            Director = director;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="director">Saved county.</param>
        /// <returns>Response.</returns>
        public DirectorResponse(Director director) : this(true, string.Empty, director)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public DirectorResponse(string message) : this(false, message, null)
        { }


    }
}
