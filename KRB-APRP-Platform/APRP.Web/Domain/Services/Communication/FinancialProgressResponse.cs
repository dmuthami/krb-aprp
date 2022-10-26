using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class FinancialProgressResponse : BaseResponse
    {
        public FinancialProgress FinancialProgress { get; set; }
        public FinancialProgressResponse(bool success, string message, FinancialProgress financialProgress) : base(success, message)
        {
            FinancialProgress = financialProgress;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="financialProgress">Saved road.</param>
        /// <returns>Response.</returns>
        public FinancialProgressResponse(FinancialProgress financialProgress) : this(true, string.Empty, financialProgress)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public FinancialProgressResponse(string message) : this(false, message, null) 
        { }
    }
}
