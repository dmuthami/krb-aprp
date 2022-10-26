using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class AllocationCodeUnitListResponse : BaseResponse
    {
        public IEnumerable<AllocationCodeUnit> AllocationCodeUnit { get; set; }


        private AllocationCodeUnitListResponse(bool success, string message, IEnumerable<AllocationCodeUnit> allocationCodeUnit) : base(success, message)
        {
            AllocationCodeUnit =allocationCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public AllocationCodeUnitListResponse(IEnumerable<AllocationCodeUnit> allocationCodeUnit) : this(true, string.Empty, allocationCodeUnit)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public AllocationCodeUnitListResponse(string message) : this(false, message, null)
        { }
    }
}
