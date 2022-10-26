using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class KenHARoadResponse : BaseResponse
    {
        public KenhaRoad KenhaRoad { get; set; }

       
        private KenHARoadResponse(bool success, string message, KenhaRoad kenhaRoad) : base(success, message)
        {
            KenhaRoad = kenhaRoad;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public KenHARoadResponse(KenhaRoad kenhaRoad) : this(true, string.Empty, kenhaRoad)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KenHARoadResponse(string message) : this(false, message, null)
        { }

    }
}
