using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RegionResponse : BaseResponse
    {
        public Region Region { get; set; }


        private RegionResponse(bool success, string message, Region region) : base(success, message)
        {
            Region = region;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="region">Saved road.</param>
        /// <returns>Response.</returns>
        public RegionResponse(Region region) : this(true, string.Empty, region)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RegionResponse(string message) : this(false, message, null)
        { }
    }
}
