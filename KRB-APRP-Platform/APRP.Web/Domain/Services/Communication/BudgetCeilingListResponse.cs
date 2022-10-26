using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class BudgetCeilingListResponse : BaseResponse
    {
        public IEnumerable<BudgetCeiling> BudgetCeilings { get; set; }
        private BudgetCeilingListResponse(bool success, string message, IEnumerable<BudgetCeiling> budgetCeilings) : base(success, message)
        {
            BudgetCeilings = budgetCeilings;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="budgetCeiling">Saved county.</param>
        /// <returns>Response.</returns>
        public BudgetCeilingListResponse(IEnumerable<BudgetCeiling> budgetCeilings) : this(true, string.Empty, budgetCeilings) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public BudgetCeilingListResponse(string message) : this(false, message, null) { }
    }

}
