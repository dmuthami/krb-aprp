using APRP.Web.Domain.Models;
using System.ComponentModel;

namespace APRP.Web.ViewModels
{
    public class FinancialProgressViewModel
    {
        public FinancialProgress FinancialProgress{ get; set; }
        public IEnumerable<FinancialProgress> FinancialProgressList { get; set; }
        [DisplayName("Bank Reconciliation File")]
        public IFormFile BankReconFile { get; set; }

        [DisplayName("Annual Financial Statement")]
        public IFormFile FinancialStatetement { get; set; }

    }
}
