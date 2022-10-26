using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Services.Communication
{
    public class CommunicationResponse : BaseResponse
    {
        public IActionResult IActionResult { get; set; }


        private CommunicationResponse(bool success, string message, IActionResult actionResult) : base(success, message)
        {
            IActionResult = actionResult;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public CommunicationResponse(IActionResult actionResult) : this(true, string.Empty, actionResult)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CommunicationResponse(string message) : this(false, message, null)
        { }
    }
}
