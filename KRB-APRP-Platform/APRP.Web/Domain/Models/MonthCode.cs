using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class MonthCode
    {
        [Key]
        public long ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
