using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class MessageOut
    {
        //[ExplicitKey]
        public long ID { get; set; }
        [MaxLength(80)]
        public string MessageTo { get; set; }
        public string MessageText { get; set; }
        public int IsSent { get; set; }
        public int IsRead { get; set; }
    }
}
