using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadWorkBudgetLineResponse : BaseResponse
    {
        public RoadWorkBudgetLine RoadWorkBudgetLine { get; set; }


        private RoadWorkBudgetLineResponse(bool success, string message, RoadWorkBudgetLine roadWorkBudgetLine) : base(success, message)
        {
            RoadWorkBudgetLine = roadWorkBudgetLine;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="roadWorkBudgetLine">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadWorkBudgetLineResponse(RoadWorkBudgetLine roadWorkBudgetLine) : this(true, string.Empty, roadWorkBudgetLine)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadWorkBudgetLineResponse(string message) : this(false, message, null) 
        { }

    }
}
