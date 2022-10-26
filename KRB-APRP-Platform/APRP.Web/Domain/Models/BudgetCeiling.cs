namespace APRP.Web.Domain.Models
{
    public class BudgetCeiling
    {
        public long ID { get; set; }
        public Authority Authority { get; set; }
        public long AuthorityId { get; set; }
        public double Amount { get; set; }
        public string AdditionalInfo { get; set; }
        public long BudgetCeilingHeaderId { get; set; }
        public BudgetCeilingHeader RoadWorkBudgetHeader { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }


    }
}
