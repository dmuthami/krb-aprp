using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class QuarterCodeList
    {
        public long ID { get; set; }

        [Display(Name = "Name of Quarter")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int ReferenceID { get; set; }

    }
}
