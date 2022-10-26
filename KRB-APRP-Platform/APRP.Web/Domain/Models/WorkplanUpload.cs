using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkplanUpload
    {
        [Key]
        public long ID { get; set; }
        public long? FinancialYearId { get; set; }
        public FinancialYear FinancialYear { get; set; }
        public long? AuthorityId{ get; set; }
        public Authority Authority{ get; set; }
        public double UploadBudget{ get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public int WorkplansCreated { get; set; }
        public DateTime WorkplansCreatedDate { get; set; }
        public string WorkplansCreatedBy { get; set; }
        public List<WorkplanUploadSection> WorkplanUploadSections { get; set; }

    }
}
