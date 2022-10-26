using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RevenueCollectionCodeUnitTypeResponse : BaseResponse
    {
        public RevenueCollectionCodeUnitType RevenueCollectionCodeUnitType { get; set; }


        private RevenueCollectionCodeUnitTypeResponse(bool success, string message, RevenueCollectionCodeUnitType revenueCollectionCodeUnitType) : base(success, message)
        {
            RevenueCollectionCodeUnitType = revenueCollectionCodeUnitType;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RevenueCollectionCodeUnitType">Saved RevenueCollectionCodeUnitType.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionCodeUnitTypeResponse(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType) : this(true, string.Empty, revenueCollectionCodeUnitType)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionCodeUnitTypeResponse(string message) : this(false, message, null)
        { }
    }
}
