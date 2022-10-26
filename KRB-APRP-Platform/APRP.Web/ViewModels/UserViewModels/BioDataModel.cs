using System.ComponentModel.DataAnnotations;

namespace APRP.Web.ViewModels.UserViewModels
{
    public class BioDataModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "National ID/KRA Pin"), MaxLength(25)]
        public string Id_or_Pin { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Surburb"), MaxLength(25)]
        public string Surburb { get; set; }


        [Display(Name = "House Number"), MaxLength(25)]
        public string HouseNumber { get; set; }


        [Display(Name = "Street Name"), MaxLength(25)]
        public string Street_Name { get; set; }

        [Display(Name = "Postal Address"), MaxLength(25)]
        public string Postal_Address { get; set; }

        [Display(Name = "Postal Code"), MaxLength(25)]
        public string Postal_Code { get; set; }

        [Display(Name = "Surname"), MaxLength(25)]
        public string Surname { get; set; }

        [Display(Name = "Other Names"), MaxLength(25)]
        public string Other_Names { get; set; }

        [Display(Name = "Full Name")]
        public string Full_Name { get; set; }

        [Display(Name = "kra_pin")]
        public string KRA_Pin { get; set; }
    }
}
