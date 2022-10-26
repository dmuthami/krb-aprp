using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class ComplaintResponse : BaseResponse
    {
        public Complaint Complaint { get; set; }
        private ComplaintResponse(bool success, string message, Complaint complaint) : base(success, message) {
            Complaint = complaint;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="county">Saved county.</param>
        /// <returns>Response.</returns>
        public ComplaintResponse(Complaint complaint) : this(true, string.Empty, complaint)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public ComplaintResponse(string message) : this(false, message, null)
        { }


    }
}
