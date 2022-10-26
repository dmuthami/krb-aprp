using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class SurfaceTypeUnPavedResponse : BaseResponse
    {
        public SurfaceTypeUnPaved SurfaceTypeUnPaved { get; set; }


        private SurfaceTypeUnPavedResponse(bool success, string message, SurfaceTypeUnPaved surfaceTypeUnPaved) : base(success, message)
        {
            SurfaceTypeUnPaved = surfaceTypeUnPaved;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public SurfaceTypeUnPavedResponse(SurfaceTypeUnPaved surfaceTypeUnPaved) : this(true, string.Empty, surfaceTypeUnPaved)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SurfaceTypeUnPavedResponse(string message) : this(false, message, null)
        { }
    }
}
