using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class ItemActivityGroupViewModel
    {
        public IEnumerable<ItemActivityUnitCost> ItemActivityUnitCostList { get; set; }

        public IEnumerable<ItemActivityGroup> ItemActivityGroupList { get; set; }

        public long ItemActivityGroupId { get; set; }

        public string Referer { get; set; }
    }
}
