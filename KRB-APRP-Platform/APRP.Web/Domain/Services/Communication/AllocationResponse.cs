using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class AllocationResponse : BaseResponse
    {
        public Allocation Allocation { get; set; }


        private AllocationResponse(bool success, string message, Allocation allocation) : base(success, message)
        {
            Allocation = allocation;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="Allocation">Saved Allocation.</param>
        /// <returns>Response.</returns>
        public AllocationResponse(Allocation allocation) : this(true, string.Empty, allocation)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public AllocationResponse(string message) : this(false, message, null)
        { }
    }
}
