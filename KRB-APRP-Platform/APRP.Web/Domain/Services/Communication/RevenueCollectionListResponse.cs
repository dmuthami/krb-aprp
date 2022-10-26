using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RevenueCollectionListResponse : BaseResponse
    {
        public IEnumerable<RevenueCollection> RevenueCollection { get; set; }


        private RevenueCollectionListResponse(bool success, string message, IEnumerable<RevenueCollection> revenueCollection) : base(success, message)
        {
            RevenueCollection =revenueCollection;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionListResponse(IEnumerable<RevenueCollection> revenueCollection) : this(true, string.Empty, revenueCollection)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionListResponse(string message) : this(false, message, null)
        { }
    }
}
