using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services.Communication
{
    public class KeRRARoadResponse : BaseResponse
    {
        public KerraRoad KerraRoad { get; set; }

       
        private KeRRARoadResponse(bool success, string message, KerraRoad kerraRoad) : base(success, message)
        {
            KerraRoad = kerraRoad;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public KeRRARoadResponse(KerraRoad kerraRoad) : this(true, string.Empty, kerraRoad)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KeRRARoadResponse(string message) : this(false, message, null)
        { }

    }
}
