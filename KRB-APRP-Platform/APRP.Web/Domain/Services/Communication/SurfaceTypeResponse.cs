using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class SurfaceTypeResponse : BaseResponse
    {
        public SurfaceType SurfaceType { get; set; }


        private SurfaceTypeResponse(bool success, string message, SurfaceType surfaceType) : base(success, message)
        {
            SurfaceType = surfaceType;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public SurfaceTypeResponse(SurfaceType surfaceType) : this(true, string.Empty, surfaceType)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SurfaceTypeResponse(string message) : this(false, message, null)
        { }
    }
}
