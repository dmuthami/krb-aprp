using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class QuarterCodeUnitListResponse : BaseResponse
    {
        public IEnumerable<QuarterCodeUnit> QuarterCodeUnit { get; set; }


        private QuarterCodeUnitListResponse(bool success, string message, IEnumerable<QuarterCodeUnit> quarterCodeUnit) : base(success, message)
        {
            QuarterCodeUnit =quarterCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public QuarterCodeUnitListResponse(IEnumerable<QuarterCodeUnit> quarterCodeUnit) : this(true, string.Empty, quarterCodeUnit)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public QuarterCodeUnitListResponse(string message) : this(false, message, null)
        { }
    }
}
