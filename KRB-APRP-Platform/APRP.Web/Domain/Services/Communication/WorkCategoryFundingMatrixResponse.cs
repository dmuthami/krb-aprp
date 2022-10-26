using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkCategoryFundingMatrixResponse : BaseResponse
    {
        public WorkCategoryFundingMatrix WorkCategoryFundingMatrix { get; set; }


        private WorkCategoryFundingMatrixResponse(bool success, string message, WorkCategoryFundingMatrix workCategoryFundingMatrix) : base(success, message)
        {
            WorkCategoryFundingMatrix = workCategoryFundingMatrix;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="WorkCategoryFundingMatrix">Saved WorkCategoryFundingMatrix.</param>
        /// <returns>Response.</returns>
        public WorkCategoryFundingMatrixResponse(WorkCategoryFundingMatrix workCategoryFundingMatrix) : this(true, string.Empty, workCategoryFundingMatrix)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkCategoryFundingMatrixResponse(string message) : this(false, message, null)
        { }
    }
}
