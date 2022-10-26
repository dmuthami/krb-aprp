using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class BudgetCeilingResponse : BaseResponse
    {
        public BudgetCeiling BudgetCeiling { get; set; }
        private BudgetCeilingResponse(bool success, string message, BudgetCeiling budgetCeiling) : base(success, message)
        {
            BudgetCeiling = budgetCeiling;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="budgetCeiling">Saved county.</param>
        /// <returns>Response.</returns>
        public BudgetCeilingResponse(BudgetCeiling budgetCeiling) : this(true, string.Empty, budgetCeiling) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public BudgetCeilingResponse(string message) : this(false, message, null) { }
    }

}
