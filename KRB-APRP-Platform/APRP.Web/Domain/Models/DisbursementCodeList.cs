using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class DisbursementCodeList
    {
        [Key]
        public long ID { get; set; }

        [StringLength(70), Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [StringLength(100), Display(Name = "Description")]
        public string Name { get; set; }

        [StringLength(100), Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public Nullable<double> Percent { get; set; }

        public bool Display { get; set; }

        public int Order { get; set; }

        public Nullable<int> SNo { get; set; }

        public Nullable<long> AuthorityId { get; set; }
        public Nullable<int> ReleaseOrder { get; set; }
    }
}
