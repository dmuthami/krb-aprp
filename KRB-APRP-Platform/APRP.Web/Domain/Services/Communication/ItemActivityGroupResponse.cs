using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ItemActivityGroupResponse : BaseResponse
    {
        public ItemActivityGroup ItemActivityGroup;
        private ItemActivityGroupResponse(bool success, string message, ItemActivityGroup itemActivityGroup) : base(success, message)
        {
            ItemActivityGroup = itemActivityGroup;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="itemActivityGroup">Saved county.</param>
        /// <returns>Response.</returns>
        public ItemActivityGroupResponse(ItemActivityGroup itemActivityGroup) : this(true, string.Empty, itemActivityGroup) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public ItemActivityGroupResponse(string message) : this(false, message, null) { }
    }
}
