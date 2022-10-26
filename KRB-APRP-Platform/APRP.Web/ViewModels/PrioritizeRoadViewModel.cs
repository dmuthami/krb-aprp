using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class PrioritizeRoadViewModel
    {
        public Road Road { get; set; }

        public RoadCondition RoadCondition { get; set; }

        public RoadPrioritization RoadPrioritization { get; set; }

        public string Referer { get; set; }
        public ARICSYear ARICSYear { get; set; }
        public int ARICSYearId { get; set; }
    }
}
