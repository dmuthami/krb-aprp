using APRP.Web.ViewModels;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkCategoryAllocationMatrixViewModelListResponse : BaseResponse
    {
        public IEnumerable<WorkCategoryAllocationMatrixViewModel> WorkCategoryAllocationMatrixViewModel { get; set; }


        private WorkCategoryAllocationMatrixViewModelListResponse(bool success, string message, IEnumerable<WorkCategoryAllocationMatrixViewModel> workCategoryAllocationMatrixViewModel) : base(success, message)
        {
            WorkCategoryAllocationMatrixViewModel =workCategoryAllocationMatrixViewModel;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public WorkCategoryAllocationMatrixViewModelListResponse(IEnumerable<WorkCategoryAllocationMatrixViewModel> workCategoryAllocationMatrixViewModel) : this(true, string.Empty, workCategoryAllocationMatrixViewModel)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public WorkCategoryAllocationMatrixViewModelListResponse(string message) : this(false, message, null)
        { }
    }
}
