using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class WorkCategoryResponse : BaseResponse
    {
        public WorkCategory WorkCategory;
        private WorkCategoryResponse(bool success, string message, WorkCategory workCategory) : base(success, message)
        {
            WorkCategory = workCategory;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="workCategory">Saved county.</param>
        /// <returns>Response.</returns>
        public WorkCategoryResponse(WorkCategory workCategory) : this(true, string.Empty, workCategory) { }

        /// <summary>
        /// Creates a fail response.
        /// </summary>
        /// <param name="message">Saved county.</param>
        /// <returns>Response.</returns>
        public WorkCategoryResponse(string message) : this(false, message, null) { }
    }
}

