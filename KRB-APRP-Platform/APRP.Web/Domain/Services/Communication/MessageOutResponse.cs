using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class MessageOutResponse : BaseResponse
    {
        public MessageOut MessageOut { get; set; }


        private MessageOutResponse(bool success, string message, MessageOut messageOut) : base(success, message)
        {
            MessageOut = messageOut;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="MessageOut">Saved MessageOut.</param>
        /// <returns>Response.</returns>
        public MessageOutResponse(MessageOut messageOut) : this(true, string.Empty, messageOut)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public MessageOutResponse(string message) : this(false, message, null)
        { }
    }
}
