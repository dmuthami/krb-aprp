using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ShoulderSurfaceTypePavedListResponse : BaseResponse
    {
        public IEnumerable<ShoulderSurfaceTypePaved> ShoulderSurfaceTypePaved { get; set; }


        private ShoulderSurfaceTypePavedListResponse(bool success, string message, IEnumerable<ShoulderSurfaceTypePaved> shoulderSurfaceTypePaved) : base(success, message)
        {
            ShoulderSurfaceTypePaved = shoulderSurfaceTypePaved;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ShoulderSurfaceTypePavedListResponse(IEnumerable<ShoulderSurfaceTypePaved> shoulderSurfaceTypePaved) : this(true, string.Empty, shoulderSurfaceTypePaved)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ShoulderSurfaceTypePavedListResponse(string message) : this(false, message, null)
        { }
    }
}
