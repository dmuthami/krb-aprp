using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ItemActivityUnitCostResponse : BaseResponse
    {
        public ItemActivityUnitCost ItemActivityUnitCost;
        private ItemActivityUnitCostResponse(bool success, string message, ItemActivityUnitCost itemActivityUnitCost) : base(success, message)
        {
            ItemActivityUnitCost = itemActivityUnitCost;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="itemActivityUnitCost">Saved county.</param>
        /// <returns>Response.</returns>
        public ItemActivityUnitCostResponse(ItemActivityUnitCost itemActivityUnitCost) : this(true, string.Empty, itemActivityUnitCost) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public ItemActivityUnitCostResponse(string message) : this(false, message, null) { }
    }
}

