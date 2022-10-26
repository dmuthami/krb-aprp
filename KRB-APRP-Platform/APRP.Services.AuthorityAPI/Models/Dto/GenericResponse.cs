using Microsoft.AspNetCore.Mvc;

namespace APRP.Services.AuthorityAPI.Models.Dto
{
    public class GenericResponse : BaseResponse
    {
        public IActionResult IActionResult { get; set; }


        private GenericResponse(bool success, string message, IActionResult actionResult) : base(success, message)
        {
            IActionResult = actionResult;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <returns>Response.</returns>
        public GenericResponse(IActionResult actionResult) : this(true, string.Empty, actionResult)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public GenericResponse(string message) : this(false, message, null)
        { }
    }
}
