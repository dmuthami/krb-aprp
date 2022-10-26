using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class BudgetCeilingHeaderResponse : BaseResponse
    {

        public BudgetCeilingHeader BudgetCeilingHeader { get; set; }


        private BudgetCeilingHeaderResponse(bool success, string message, BudgetCeilingHeader budgetCeilingHeader) : base(success, message)
        {
            BudgetCeilingHeader = budgetCeilingHeader;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="roadWorkBudgetHeader">Saved road.</param>
        /// <returns>Response.</returns>
        public BudgetCeilingHeaderResponse(BudgetCeilingHeader budgetCeilingHeader) : this(true, string.Empty, budgetCeilingHeader)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public BudgetCeilingHeaderResponse(string message) : this(false, message, null)
        { }

    }
}
