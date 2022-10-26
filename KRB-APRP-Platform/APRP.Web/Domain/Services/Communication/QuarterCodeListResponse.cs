using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class QuarterCodeListResponse : BaseResponse
    {
        public QuarterCodeList QuarterCodeList { get; set; }


        private QuarterCodeListResponse(bool success, string message, QuarterCodeList quarterCodeList) : base(success, message)
        {
            QuarterCodeList = quarterCodeList;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="QuarterCodeList">Saved QuarterCodeList.</param>
        /// <returns>Response.</returns>
        public QuarterCodeListResponse(QuarterCodeList quarterCodeList) : this(true, string.Empty, quarterCodeList)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public QuarterCodeListResponse(string message) : this(false, message, null)
        { }
    }
}
