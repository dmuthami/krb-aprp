using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RevenueCollectionCodeUnitListResponse : BaseResponse
    {
        public IEnumerable<RevenueCollectionCodeUnit> RevenueCollectionCodeUnit { get; set; }


        private RevenueCollectionCodeUnitListResponse(bool success, string message, IEnumerable<RevenueCollectionCodeUnit> revenueCollectionCodeUnit) : base(success, message)
        {
            RevenueCollectionCodeUnit =revenueCollectionCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionCodeUnitListResponse(IEnumerable<RevenueCollectionCodeUnit> revenueCollectionCodeUnit) : this(true, string.Empty, revenueCollectionCodeUnit)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionCodeUnitListResponse(string message) : this(false, message, null)
        { }
    }
}
