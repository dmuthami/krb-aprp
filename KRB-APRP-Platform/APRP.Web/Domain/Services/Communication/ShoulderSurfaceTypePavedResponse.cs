using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ShoulderSurfaceTypePavedResponse : BaseResponse
    {
        public ShoulderSurfaceTypePaved ShoulderSurfaceTypePaved { get; set; }


        private ShoulderSurfaceTypePavedResponse(bool success, string message, ShoulderSurfaceTypePaved shoulderSurfaceTypePaved) : base(success, message)
        {
            ShoulderSurfaceTypePaved = shoulderSurfaceTypePaved;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public ShoulderSurfaceTypePavedResponse(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved) : this(true, string.Empty, shoulderSurfaceTypePaved)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ShoulderSurfaceTypePavedResponse(string message) : this(false, message, null)
        { }
    }
}
