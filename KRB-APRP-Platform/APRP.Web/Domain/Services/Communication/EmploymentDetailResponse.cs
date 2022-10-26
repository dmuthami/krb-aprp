using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class EmploymentDetailResponse : BaseResponse
    {
        public EmploymentDetail EmploymentDetail{ get; set; }
        private EmploymentDetailResponse(bool success, string message, EmploymentDetail employmentDetail) : base(success, message) {
            EmploymentDetail = employmentDetail;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="employmentDetail">Saved county.</param>
        /// <returns>Response.</returns>
        public EmploymentDetailResponse(EmploymentDetail employmentDetail) : this(true, string.Empty, employmentDetail)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public EmploymentDetailResponse(string message) : this(false, message, null)
        { }


    }
}
