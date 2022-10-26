using APRP.Web.Domain.Models;
using System.ComponentModel;

namespace APRP.Web.ViewModels
{
    public class WorkplanUploadVewModel
    {
        public FinancialYear FinancialYear { get; set; }
        public Authority Authority { get; set; }
        public FundingSourceSubCode FundingSourceSubCode { get; set; }
        public WorkplanUpload WorkplanUpload { get; set; }
        public List<WorkplanUpload> WorkplanUploads { get; set; }

        public List<MissingSectionViewModel> MissingRoadSections { get; set; }
        public List<MissingWorkCategoriesViewModel> MissingCategories { get; set; }
        public List<MissingItemViewModel> MissingActivities { get; set; }
        public List<string> MissingTechnologies { get; set; }
        public double BudgetAmount { get; set; }
        public double TotalWorkplanCost { get; set; }

        [DisplayName("Approval Comments *")]
        public string Comment { get; set; }

        public int UploadApproval { get; set; }

        [DisplayName("Workplan Document")]
        public IFormFile SupportDocument { get; set; }
        public AdminOperationalActivity AdminOperationalActivity { get; set; }
        public List<AdminOperationalActivity> AdminOperationalActivities { get; set; }

    }
}
