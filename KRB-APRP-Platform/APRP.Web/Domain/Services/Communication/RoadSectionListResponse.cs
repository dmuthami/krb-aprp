using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadSectionListResponse : BaseResponse
    {

        public IEnumerable<RoadSection> RoadSectionList;

        private RoadSectionListResponse(bool success, string message, IEnumerable<RoadSection> roadSectionList) : base(success, message)
        {
            RoadSectionList = roadSectionList;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="roadSection">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadSectionListResponse(IEnumerable<RoadSection> roadSection) : this(true, string.Empty, roadSection)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadSectionListResponse(string message) : this(false, message, null)
        { }
    }
}
