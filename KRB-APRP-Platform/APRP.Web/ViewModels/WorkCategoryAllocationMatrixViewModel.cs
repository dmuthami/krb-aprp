namespace APRP.Web.ViewModels
{
    public class WorkCategoryAllocationMatrixViewModel
    {
        public int ID { get; set; }
        public long AuthorityId { get; set; }
        public long FinancialYearId { get; set; }
        public double Percent { get; set; }
        public double Amount { get; set; }
        public long WorkCategoryId { get; set; }

    }
}
