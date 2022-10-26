using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class AdminOperationalActivity
    {
        [Key]
        public long ID { get; set; }
        public long AuthorityId { get; set; }
        public Authority Authority { get; set; }
        public long FinancialYearId{ get; set; }
        public FinancialYear FinancialYear{ get; set; }
        public string ActivityGroup { get; set; }
        public string ActivityWorks { get; set; }
        public double Amount { get; set; }
        public string RoadID { get; set; }
        public string RoadSection { get; set; }
        public string ST { get; set; }
        public string KM { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }

    }
}
