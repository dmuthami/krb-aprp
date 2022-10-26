using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadWorkBudgetHeaderResponse : BaseResponse
    {

        public RoadWorkBudgetHeader RoadWorkBudgetHeader { get; set; }


        private RoadWorkBudgetHeaderResponse(bool success, string message, RoadWorkBudgetHeader roadWorkBudgetHeader) : base(success, message)
        {
            RoadWorkBudgetHeader = roadWorkBudgetHeader;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="roadWorkBudgetHeader">Saved road.</param>
        /// <returns>Response.</returns>
        public RoadWorkBudgetHeaderResponse(RoadWorkBudgetHeader roadWorkBudgetHeader) : this(true, string.Empty, roadWorkBudgetHeader)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadWorkBudgetHeaderResponse(string message) : this(false, message, null)
        { }

    }
}
