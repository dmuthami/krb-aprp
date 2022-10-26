using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class RoadWorkOperationalActivitiesBudgetResponse : BaseResponse
    {
        public RoadWorkOperationalActivitiesBudget RoadWorkOperationalActivitiesBudget { get; set; }


        private RoadWorkOperationalActivitiesBudgetResponse(bool success, string message, RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget) : 
            base(success, message)
        {
            RoadWorkOperationalActivitiesBudget = roadWorkOperationalActivitiesBudget;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="roadWorkOperationalActivitiesBudget">Saved road budget line.</param>
        /// <returns>Response.</returns>
        public RoadWorkOperationalActivitiesBudgetResponse(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget) : 
            this(true, string.Empty, roadWorkOperationalActivitiesBudget)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public RoadWorkOperationalActivitiesBudgetResponse(string message) : this(false, message, null)
        { }

    }
}
