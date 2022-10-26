using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class RoadWorkPlanViewItemModel
    {
        public Road Road { get; set; }
        public IEnumerable<FundingSource> FundingSources { get; set; }
        public IEnumerable<RoadSection> RoadSections { get; set; }
        public IEnumerable<FundType> FundTypes { get; set; }
        public IEnumerable<ExecutionMethod> ExecutionMethods { get; set; }
        public IEnumerable<WorkCategory> WorkCategories { get; set; }

    }
}
