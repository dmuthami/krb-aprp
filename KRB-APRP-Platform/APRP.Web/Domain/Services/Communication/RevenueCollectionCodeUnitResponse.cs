using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RevenueCollectionCodeUnitResponse : BaseResponse
    {
        public RevenueCollectionCodeUnit RevenueCollectionCodeUnit { get; set; }


        private RevenueCollectionCodeUnitResponse(bool success, string message, RevenueCollectionCodeUnit revenueCollectionCodeUnit) : base(success, message)
        {
            RevenueCollectionCodeUnit = revenueCollectionCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="RevenueCollectionCodeUnit">Saved RevenueCollectionCodeUnit.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionCodeUnitResponse(RevenueCollectionCodeUnit revenueCollectionCodeUnit) : this(true, string.Empty, revenueCollectionCodeUnit)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RevenueCollectionCodeUnitResponse(string message) : this(false, message, null)
        { }
    }
}
