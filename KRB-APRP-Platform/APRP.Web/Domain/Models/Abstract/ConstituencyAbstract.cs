using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models.Abstract
{
    public abstract class ConstituencyAbstract
    {
        [StringLength(70)]
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(70)]
        public string Code { get; set; }

        [StringLength(25)]
        public string AdminCode { get; set; }
    }
}
