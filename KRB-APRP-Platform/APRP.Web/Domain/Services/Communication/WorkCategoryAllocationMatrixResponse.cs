using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkCategoryAllocationMatrixResponse : BaseResponse
    {
        public WorkCategoryAllocationMatrix WorkCategoryAllocationMatrix { get; set; }


        private WorkCategoryAllocationMatrixResponse(bool success, string message, WorkCategoryAllocationMatrix workCategoryAllocationMatrix) : base(success, message)
        {
            WorkCategoryAllocationMatrix = workCategoryAllocationMatrix;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="WorkCategoryAllocationMatrix">Saved WorkCategoryAllocationMatrix.</param>
        /// <returns>Response.</returns>
        public WorkCategoryAllocationMatrixResponse(WorkCategoryAllocationMatrix workCategoryAllocationMatrix) : this(true, string.Empty, workCategoryAllocationMatrix)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkCategoryAllocationMatrixResponse(string message) : this(false, message, null)
        { }
    }
}
