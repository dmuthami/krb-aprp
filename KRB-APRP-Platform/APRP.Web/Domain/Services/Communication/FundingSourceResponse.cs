using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class FundingSourceResponse : BaseResponse
    {
        public FundingSource FundingSource { get; set; }


        private FundingSourceResponse(bool success, string message, FundingSource fundingSource) : base(success, message)
        {
            FundingSource = fundingSource;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="fundingSource">Saved road.</param>
        /// <returns>Response.</returns>
        public FundingSourceResponse(FundingSource fundingSource) : this(true, string.Empty, fundingSource)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public FundingSourceResponse(string message) : this(false, message, null) 
        { }

    }
}
