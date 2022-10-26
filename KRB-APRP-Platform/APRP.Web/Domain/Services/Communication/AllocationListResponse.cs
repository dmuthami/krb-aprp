using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class AllocationListResponse : BaseResponse
    {
        public IEnumerable<Allocation> Allocation { get; set; }


        private AllocationListResponse(bool success, string message, IEnumerable<Allocation> allocation) : base(success, message)
        {
            Allocation =allocation;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public AllocationListResponse(IEnumerable<Allocation> allocation) : this(true, string.Empty, allocation)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public AllocationListResponse(string message) : this(false, message, null)
        { }
    }
}
