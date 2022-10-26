using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class SurfaceTypeUnPavedListResponse : BaseResponse
    {
        public IEnumerable<SurfaceTypeUnPaved> SurfaceTypeUnPaved { get; set; }


        private SurfaceTypeUnPavedListResponse(bool success, string message, IEnumerable<SurfaceTypeUnPaved> surfaceTypeUnPaved) : base(success, message)
        {
            SurfaceTypeUnPaved = surfaceTypeUnPaved;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public SurfaceTypeUnPavedListResponse(IEnumerable<SurfaceTypeUnPaved> surfaceTypeUnPaved) : this(true, string.Empty, surfaceTypeUnPaved)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SurfaceTypeUnPavedListResponse(string message) : this(false, message, null)
        { }
    }
}
