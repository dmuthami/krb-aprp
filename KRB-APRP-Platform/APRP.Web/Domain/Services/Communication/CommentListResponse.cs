using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class CommentListResponse : BaseResponse
    {
        public IEnumerable<Comment> Comment { get; set; }


        private CommentListResponse(bool success, string message, IEnumerable<Comment> comment) : base(success, message)
        {
            Comment = comment;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public CommentListResponse(IEnumerable<Comment> comment) : this(true, string.Empty, comment)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CommentListResponse(string message) : this(false, message, null)
        { }
    }
}
