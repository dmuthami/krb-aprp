using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RevenueCollectionResponse : BaseResponse
    {
        public RevenueCollection RevenueCollection { get; set; }


        private RevenueCollectionResponse(bool success, string message, RevenueCollection revenueCollection) : base(success, message)
        {
            RevenueCollection = revenueCollection;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RevenueCollection">Saved RevenueCollection.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionResponse(RevenueCollection revenueCollection) : this(true, string.Empty, revenueCollection)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionResponse(string message) : this(false, message, null)
        { }
    }
}
