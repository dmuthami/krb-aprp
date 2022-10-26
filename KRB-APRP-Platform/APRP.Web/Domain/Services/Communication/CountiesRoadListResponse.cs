using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services.Communication
{
    public class CountiesRoadListResponse : BaseResponse
    {
        public IEnumerable<CountiesRoad> CountiesRoads { get; set; }
        private CountiesRoadListResponse(bool success, string message, IEnumerable<CountiesRoad> countiesRoads) : base(success, message)
        {
            CountiesRoads = countiesRoads;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Saved category.</param>
        /// <returns>Response.</returns>
        public CountiesRoadListResponse(IEnumerable<CountiesRoad> countiesRoads) : this(true, string.Empty, countiesRoads)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CountiesRoadListResponse(string message) : this(false, message, null)
        { }
    }
}
