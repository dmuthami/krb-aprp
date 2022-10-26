using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models.Abstract
{
    public abstract class CountyAbstract
    {
        [StringLength(70)]
        public string Name { get; set; }

        [StringLength(3)]
        public string Code { get; set; }

        public int CountyNumber { get; set; }
    }
}
