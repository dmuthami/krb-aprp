using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkCategoryAllocationMatrixListResponse : BaseResponse
    {
        public IEnumerable<WorkCategoryAllocationMatrix> WorkCategoryAllocationMatrix { get; set; }


        private WorkCategoryAllocationMatrixListResponse(bool success, string message, IEnumerable<WorkCategoryAllocationMatrix> workCategoryAllocationMatrix) : base(success, message)
        {
            WorkCategoryAllocationMatrix =workCategoryAllocationMatrix;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public WorkCategoryAllocationMatrixListResponse(IEnumerable<WorkCategoryAllocationMatrix> workCategoryAllocationMatrix) : this(true, string.Empty, workCategoryAllocationMatrix)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkCategoryAllocationMatrixListResponse(string message) : this(false, message, null)
        { }
    }
}
