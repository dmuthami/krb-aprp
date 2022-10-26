using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class FinancialProgress
    {
        [Key]
        public long ID { get; set; }
        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }
        public long  FinancialYearId { get; set; }
        public FinancialYear  FinancialYear { get; set; }
        [Display(Name = "Opening Balance")]
        public double OpeningBalance { get; set; }
        [Display(Name = "Closing Balance")]
        public double ClosingBalance { get; set; }
        [Display(Name = "Receipts")]
        public double FinancialReceipts { get; set; }
        [Display(Name = "Receipts Reference")]
        public double FinancialReceiptReference { get; set; }
        [Display(Name = "Expenditure")]
        public double FinancialExpenditure { get; set; }
        public long? QuarterCodeListId { get; set; }
        public QuarterCodeList QuarterCodeList { get; set; }
        public long MonthCodeId{ get; set; }
        public MonthCode MonthCode{ get; set; }
        public string BankReconFileName { get; set; }
        public string AnnualFinancialStatementFileName{ get; set; }
    }
}
