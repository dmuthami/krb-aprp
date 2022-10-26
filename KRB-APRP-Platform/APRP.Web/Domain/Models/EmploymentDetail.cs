using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class EmploymentDetail
    {
        [Key]
        public long ID { get; set; }
        public long ContractId { get; set; }
        public Contract Contract { get; set; }
        public int Period { get; set; }
        public int PreviousPeriod { get; set; }

        [Display(Name = "Male Count")]
        public int MaleCount { get; set; }

        [Display(Name = "Female Count")]
        public int FemaleCount { get; set; }

        [Display(Name = "Male Person Days")]
        public int MaleMandays { get; set; }

        [Display(Name = "Female Person days")]
        public int FemaleMandays { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
