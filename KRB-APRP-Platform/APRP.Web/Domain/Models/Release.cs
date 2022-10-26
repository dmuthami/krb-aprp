using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class Release
    {
        public long ID { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Date")]
        public DateTime ReleaseDate { get; set; }

         [Display(Name = "CHQ.NO.")]
        public string ChequeNo { get; set; }

        [Display(Name = "DETAILS/PAYEE")]
        public string DetailsOrPayee { get; set; }

        [Display(Name = "Amount")]
        public double ReleaseAmount { get; set; }

        //public ICollection<Disbursement> Disbursements { get; set; }
        public IList<DisbursementRelease> DisbursementReleases { get; set; }
    }
}
