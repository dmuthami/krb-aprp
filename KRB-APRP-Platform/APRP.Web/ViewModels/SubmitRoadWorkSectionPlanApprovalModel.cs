using APRP.Web.Domain.Models;
using System.ComponentModel;

namespace APRP.Web.ViewModels
{
    public class SubmitRoadWorkSectionPlanApprovalModel
    {
        public FinancialYear FinancialYear { get; set; }
        public Authority Authority { get; set; }

        [DisplayName("Approval Comments *")]
        public string Comment { get; set; }

        public string WorkplanApproval { get; set; }
        public WorkplanApprovalBatch WorkplanApprovalBatch { get; set; }

        [DisplayName("Approval Supporting Document")]
        public IFormFile SupportDocument { get; set; }

    }
}
