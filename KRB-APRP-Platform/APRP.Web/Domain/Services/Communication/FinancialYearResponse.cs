using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class FinancialYearResponse : BaseResponse
    {
        public FinancialYear FinancialYear { get; set; }
        public FinancialYearResponse(bool success, string message, FinancialYear financialYear) : base(success, message)
        {
            FinancialYear = financialYear;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="financialYear">Saved road.</param>
        /// <returns>Response.</returns>
        public FinancialYearResponse(FinancialYear financialYear) : this(true, string.Empty, financialYear)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public FinancialYearResponse(string message) : this(false, message, null) 
        { }
    }
}
