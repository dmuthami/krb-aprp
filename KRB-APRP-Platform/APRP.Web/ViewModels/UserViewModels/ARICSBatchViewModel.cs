using APRP.Web.Domain.Models;
using APRP.Web.Domain.Models.History;

namespace APRP.Web.ViewModels.UserViewModels
{
    public class ARICSBatchViewModel
    {
        public long id { get; set; }

        //ARICS Batch
        public ARICSBatch ARICSBatch { get; set; }

        public ARICSMasterApproval ARICSMasterApproval { get; set; }

        public Authority Authority { get; set; }

        public ARICSYear ARICSYear { get; set; }

        public int Year { get; set; }

        public IList<ARICSData> ARICSDatas { get; set; }

        public IList<RoadSection> RoadSections { get; set; }

        /// <summary>
        /// Road Section fields
        /// </summary>
        public string sectionname { get; set; }

        public string sectionid { get; set; }
        public string roadnumber { get; set; }



        /// <summary>
        /// ARICS Master approval
        /// </summary>
        public string batchno { get; set; }

        public string description { get; set; }

        public long authorityid { get; set; }

        public int aricsyearid { get; set; }

        public long roadsectionid { get; set; }

        public long aricsmasterapprovalid { get; set; }

        /// <summary>
        /// Approvals
        /// </summary>
        public ARICSApproval ARICSApproval { get; set; }

        public IList<ARICSApprovalh> ARICSApprovalh { get; set; }

        public int NextStatus { get; set; }

        public int ResetStatus { get; set; }
    }
}
