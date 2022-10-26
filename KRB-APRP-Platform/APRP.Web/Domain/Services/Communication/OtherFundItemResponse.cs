using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class OtherFundItemResponse : BaseResponse
    {
        public OtherFundItem OtherFundItem { get; set; }


        private OtherFundItemResponse(bool success, string message, OtherFundItem otherFundItem) : base(success, message)
        {
            OtherFundItem = otherFundItem;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="OtherFundItem">Saved OtherFundItem.</param>
        /// <returns>Response.</returns>
        public OtherFundItemResponse(OtherFundItem otherFundItem) : this(true, string.Empty, otherFundItem)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public OtherFundItemResponse(string message) : this(false, message, null)
        { }
    }
}
