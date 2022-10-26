using APRP.Web.Domain.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APRP.Web.ViewModels.COSTES
{
    public enum RegionalCode
    {
        [Display(Name = "Nairobi,Mombasa,Kisumu")]
        [Description("Nairobi,Mombasa,Kisumu")]
        NairobiKisumuMombasa = 0,

        [Display(Name = "Other Regions")]
        [Description("Other Regions")]
        OtherRegions = 1
    }
    public class COSTESTViewModel
    {
        public IEnumerable<GetInstructutedWorkItemsViewModel> GetInstructutedWorkItemsViewModel { get; set; }

        public ARICSYear ARICSYear { get; set; }

        public RegionalCode RegionalCode { get; set; }

        public int RegionCode { get; set; }

        public CostesRegion CostesRegion { get; set; }

        public FinancialYear FinancialYear { get; set; }
    }
}
