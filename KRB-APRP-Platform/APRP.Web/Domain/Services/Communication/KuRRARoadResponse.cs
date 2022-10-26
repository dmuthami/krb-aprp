using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class KuRRARoadResponse : BaseResponse
    {
        public KuraRoad KuraRoad { get; set; }

       
        private KuRRARoadResponse(bool success, string message, KuraRoad kuraRoad) : base(success, message)
        {
            KuraRoad = kuraRoad;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public KuRRARoadResponse(KuraRoad kuraRoad) : this(true, string.Empty, kuraRoad)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KuRRARoadResponse(string message) : this(false, message, null)
        { }

    }
}
