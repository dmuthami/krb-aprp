using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class AllocationCodeUnitResponse : BaseResponse
    {
        public AllocationCodeUnit AllocationCodeUnit { get; set; }


        private AllocationCodeUnitResponse(bool success, string message, AllocationCodeUnit allocationCodeUnit) : base(success, message)
        {
            AllocationCodeUnit = allocationCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="AllocationCodeUnit">Saved AllocationCodeUnit.</param>
        /// <returns>Response.</returns>
        public AllocationCodeUnitResponse(AllocationCodeUnit allocationCodeUnit) : this(true, string.Empty, allocationCodeUnit)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public AllocationCodeUnitResponse(string message) : this(false, message, null)
        { }
    }
}
