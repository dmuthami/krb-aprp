using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class ReportsViewModel
    {
        public Authority Authority { get; set; }
        public SubmitNewWorkplanModel SubmitNewWorkplanModel { get; set; }

        public UserAccessList UserAccessList { get; set; }

        public ARICSYear ARICSYear { get; set; }

    }

}
