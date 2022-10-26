using APRP.Web.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace APRP.Web.ViewModels
{
    public class PackageQuantityEditViewModel
    {
        public PlanActivity PlanActivity { get; set; }

        public long workPackageId { get; set; }

        public int EditFlag { get; set; }

        [Display(Name="Progress Quantity")]
        public int ProgressQuantity { get; set; }

    }
}
