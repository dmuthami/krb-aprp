using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class CSAllocationtResponse : BaseResponse
    {
        public CSAllocation CSAllocation { get; set; }


        private CSAllocationtResponse(bool success, string message, CSAllocation cSAllocation) : base(success, message)
        {
            CSAllocation = cSAllocation;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="CSAllocation">Saved CSAllocation.</param>
        /// <returns>Response.</returns>
        public CSAllocationtResponse(CSAllocation cSAllocation) : this(true, string.Empty, cSAllocation)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CSAllocationtResponse(string message) : this(false, message, null)
        { }
    }
}
