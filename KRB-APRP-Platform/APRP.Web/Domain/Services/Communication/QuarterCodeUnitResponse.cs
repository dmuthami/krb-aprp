using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class QuarterCodeUnitResponse : BaseResponse
    {
        public QuarterCodeUnit QuarterCodeUnit { get; set; }


        private QuarterCodeUnitResponse(bool success, string message, QuarterCodeUnit quarterCodeUnit) : base(success, message)
        {
            QuarterCodeUnit = quarterCodeUnit;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="QuarterCodeUnit">Saved QuarterCodeUnit.</param>
        /// <returns>Response.</returns>
        public QuarterCodeUnitResponse(QuarterCodeUnit quarterCodeUnit) : this(true, string.Empty, quarterCodeUnit)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public QuarterCodeUnitResponse(string message) : this(false, message, null)
        { }
    }
}
