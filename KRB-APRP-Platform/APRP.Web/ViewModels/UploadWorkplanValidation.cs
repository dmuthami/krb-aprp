using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class UploadWorkplanValidation
    {
        public bool FileIsValid { get; set; }
        public List<WorkplanUploadSection> WorkplanUploadSections { get; set; }

        public string programmeYear { get; set; }
        public string programmeCGAgencyName { get; set; }
        public string programmeCGAgencyCode { get; set; }
        public double programmeUploadBudget { get; set; }

    }
}
