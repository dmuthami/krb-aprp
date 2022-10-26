using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class PlanActivityViewModel
    {
        public long RoadId { get; set; }
        public IEnumerable<PlanActivity> PlanActivities { get; set; }
    }
}
