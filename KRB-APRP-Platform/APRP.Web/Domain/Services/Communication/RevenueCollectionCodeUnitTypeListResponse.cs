using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RevenueCollectionCodeUnitTypeListResponse : BaseResponse
    {
        public IEnumerable<RevenueCollectionCodeUnitType> RevenueCollectionCodeUnitType { get; set; }


        private RevenueCollectionCodeUnitTypeListResponse(bool success, string message, IEnumerable<RevenueCollectionCodeUnitType> revenueCollectionCodeUnitType) : base(success, message)
        {
            RevenueCollectionCodeUnitType =revenueCollectionCodeUnitType;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionCodeUnitTypeListResponse(IEnumerable<RevenueCollectionCodeUnitType> revenueCollectionCodeUnitType) : this(true, string.Empty, revenueCollectionCodeUnitType)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionCodeUnitTypeListResponse(string message) : this(false, message, null)
        { }
    }
}
