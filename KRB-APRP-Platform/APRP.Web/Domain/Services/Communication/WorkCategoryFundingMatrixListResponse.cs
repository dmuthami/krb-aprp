using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkCategoryFundingMatrixListResponse : BaseResponse
    {
        public IEnumerable<WorkCategoryFundingMatrix> WorkCategoryFundingMatrix { get; set; }


        private WorkCategoryFundingMatrixListResponse(bool success, string message, IEnumerable<WorkCategoryFundingMatrix> workCategoryFundingMatrix) : base(success, message)
        {
            WorkCategoryFundingMatrix =workCategoryFundingMatrix;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public WorkCategoryFundingMatrixListResponse(IEnumerable<WorkCategoryFundingMatrix> workCategoryFundingMatrix) : this(true, string.Empty, workCategoryFundingMatrix)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkCategoryFundingMatrixListResponse(string message) : this(false, message, null)
        { }
    }
}
