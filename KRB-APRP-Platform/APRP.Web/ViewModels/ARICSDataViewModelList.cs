using APRP.Web.Domain.Models;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.ViewModels
{
    public class ARICSDataViewModelList
    {
        public IList<ARICSData> ARICSDatas { get; set; }
        public IEnumerable<Road> RoadsWithoutArics { get; set; }
        public IList<ARICSYear> ARICSYears { get; set; }
        public ARICSYear ARICSYear { get; set; }
        public int Year { get; set; }
        public Authority Authority { get; set; }
    }
}
