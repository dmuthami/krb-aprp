using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class OtherFundItemListResponse : BaseResponse
    {
        public IEnumerable<OtherFundItem> OtherFundItem { get; set; }

        private OtherFundItemListResponse(bool success, string message, IEnumerable<OtherFundItem> otherFundItem) : base(success, message)
        {
            OtherFundItem =otherFundItem;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public OtherFundItemListResponse(IEnumerable<OtherFundItem> otherFundItem) : this(true, string.Empty, otherFundItem)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public OtherFundItemListResponse(string message) : this(false, message, null)
        { }
    }
}
