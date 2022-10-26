using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class ARICSApprovalAgencyViewModel
    {
        public string Referer { get; set; }

        public Authority Authority { get; set; }

        public IList<ARICSMasterApproval> ARICSMasterApproval { get; set; }

        public ARICSYear ARICSYear { get; set; }


    }
}
