using APRP.Web.Domain.Models;
namespace APRP.Web.Domain.Services.Communication
{
    public class CountiesRoadResponse : BaseResponse
    {
        public CountiesRoad CountiesRoad { get; set; }

       
        private CountiesRoadResponse(bool success, string message, CountiesRoad countiesRoad) : base(success, message)
        {
            CountiesRoad = countiesRoad;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public CountiesRoadResponse(CountiesRoad countiesRoad) : this(true, string.Empty, countiesRoad)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CountiesRoadResponse(string message) : this(false, message, null)
        { }

    }
}
