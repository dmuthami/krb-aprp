using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class QuarterCodeListListResponse : BaseResponse
    {
        public IEnumerable<QuarterCodeList> QuarterCodeList { get; set; }


        private QuarterCodeListListResponse(bool success, string message, IEnumerable<QuarterCodeList> quarterCodeList) : base(success, message)
        {
            QuarterCodeList =quarterCodeList;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public QuarterCodeListListResponse(IEnumerable<QuarterCodeList> quarterCodeList) : this(true, string.Empty, quarterCodeList)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public QuarterCodeListListResponse(string message) : this(false, message, null)
        { }
    }
}
