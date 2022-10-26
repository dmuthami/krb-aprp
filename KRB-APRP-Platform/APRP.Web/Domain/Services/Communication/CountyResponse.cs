using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class CountyResponse : BaseResponse
    {
        public County County { get; set; }
        private CountyResponse(bool success, string message, County county) : base(success, message) {
            County = county;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="county">Saved county.</param>
        /// <returns>Response.</returns>
        public CountyResponse(County county) : this(true, string.Empty, county)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CountyResponse(string message) : this(false, message, null)
        { }


    }
}
