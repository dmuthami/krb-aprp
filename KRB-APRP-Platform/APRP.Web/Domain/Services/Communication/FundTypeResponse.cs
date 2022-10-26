using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class FundTypeResponse : BaseResponse
    {
        public FundType FundType { get; set; }


        private FundTypeResponse(bool success, string message, FundType fundType) : base(success, message)
        {
            FundType = fundType;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="fundType">Saved road.</param>
        /// <returns>Response.</returns>
        public FundTypeResponse(FundType fundType) : this(true, string.Empty, fundType)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public FundTypeResponse(string message) : this(false, message, null)
        { }

    }
}
