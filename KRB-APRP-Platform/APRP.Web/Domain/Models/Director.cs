using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Director
    {
        [Key]
        public long ID { get; set; }
        [Display(Name ="ID Card Number")]
        public string IdCardNumber { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public long ContractorId { get; set; }
        public Contractor Contractor { get; set; }
    }
}
