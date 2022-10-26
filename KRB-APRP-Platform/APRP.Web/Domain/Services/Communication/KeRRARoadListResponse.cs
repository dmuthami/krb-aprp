using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class KeRRARoadListResponse : BaseResponse
    {
        public IEnumerable<KerraRoad> KerraRoads { get; set; }
        private KeRRARoadListResponse(bool success, string message, IEnumerable<KerraRoad> kerraRoads) : base(success, message)
        {
            KerraRoads = kerraRoads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public KeRRARoadListResponse(IEnumerable<KerraRoad> kerraRoads) : this(true, string.Empty, kerraRoads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public KeRRARoadListResponse(string message) : this(false, message, null)
        { }
    }
}
