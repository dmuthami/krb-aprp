using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class GravelRequired
    {
        public long ID { get; set; }

        [MaxLength(3)]
        public string Code { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Survey Type includes many arics entity for the specified survey type
        /// </summary>
        public ICollection<ARICS> ARICSZ { get; set; }
    }
}
