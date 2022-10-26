using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class ARICSYearViewModel
    {
        public IList<ARICSYear> ARICSYears { get; set; }
        public ARICSYear ARICSYear { get; set; }
        public int Year { get; set; }
        public Authority Authority { get; set; }

        public string Referer { get; set; }
    }
}
