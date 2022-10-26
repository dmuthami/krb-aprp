using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class CSAllocationtListResponse : BaseResponse
    {
        public IEnumerable<CSAllocation> CSAllocation { get; set; }


        private CSAllocationtListResponse(bool success, string message, IEnumerable<CSAllocation> cSAllocation) : base(success, message)
        {
            CSAllocation =cSAllocation;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public CSAllocationtListResponse(IEnumerable<CSAllocation> cSAllocation) : this(true, string.Empty, cSAllocation)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CSAllocationtListResponse(string message) : this(false, message, null)
        { }
    }
}
