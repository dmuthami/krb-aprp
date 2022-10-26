using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Contractor
    {
        [Key]
        public long ID { get; set; }
        public string IncorporationCertificateLink { get; set; }
        public string KRAPin { get; set; }
        [Display(Name="Contractor Name")]
        public string Name { get; set; }
        public string Telephone { get; set; }
        [Display(Name="Address")]
        public string POBox { get; set; }
        [Display(Name="Bank Name")]
        public string BankName { get; set; }
        [Display(Name="Bank Branch")]
        public string BankBranchName { get; set; }
        [Display(Name="Branch Code")]
        public string BankBranchCode { get; set; }
        [Display(Name ="Account Number")]
        public string BankAccountNumber { get; set; }
        [Display(Name="Email Address")]
        public string Email { get; set; }
        public string Town { get; set; }
        public IEnumerable<Director> Directors { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
