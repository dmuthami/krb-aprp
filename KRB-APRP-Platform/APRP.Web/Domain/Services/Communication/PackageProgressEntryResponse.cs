using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class PackageProgressEntryResponse : BaseResponse
    {
        public PackageProgressEntry PackageProgressEntry { get; set; }
        public PackageProgressEntryResponse(bool success, string message, PackageProgressEntry packageProgressEntry) : base(success, message)
        {
            PackageProgressEntry = packageProgressEntry;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="packageProgressEntry">Saved road.</param>
        /// <returns>Response.</returns>
        public PackageProgressEntryResponse(PackageProgressEntry packageProgressEntry) : this(true, string.Empty, packageProgressEntry)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public PackageProgressEntryResponse(string message) : this(false, message, null) 
        { }
    }
}
