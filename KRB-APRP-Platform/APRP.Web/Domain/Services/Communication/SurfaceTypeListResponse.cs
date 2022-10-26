using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class SurfaceTypeListResponse : BaseResponse
    {
        public IEnumerable<SurfaceType> SurfaceType { get; set; }


        private SurfaceTypeListResponse(bool success, string message, IEnumerable<SurfaceType> surfaceType) : base(success, message)
        {
            SurfaceType = surfaceType;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public SurfaceTypeListResponse(IEnumerable<SurfaceType> surfaceType) : this(true, string.Empty, surfaceType)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SurfaceTypeListResponse(string message) : this(false, message, null)
        { }
    }
}
