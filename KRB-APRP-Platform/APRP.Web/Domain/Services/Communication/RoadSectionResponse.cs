using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSectionResponse : BaseResponse
    {

        public RoadSection RoadSection;

        private RoadSectionResponse(bool success, string message, RoadSection roadSection) : base(success, message)
        {
            RoadSection = roadSection;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="roadSection">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadSectionResponse(RoadSection roadSection) : this(true, string.Empty, roadSection)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSectionResponse(string message) : this(false, message, null)
        { }
    }
}
