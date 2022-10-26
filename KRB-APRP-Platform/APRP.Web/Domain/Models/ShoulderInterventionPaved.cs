using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class ShoulderInterventionPaved
    {
        public long ID { get; set; }

        [MaxLength(3)]
        public string Code { get; set; }

        [MaxLength(25)]
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Survey Type includes many arics entity for the specified survey type
        /// </summary>
        public ICollection<ARICS> ARICSZ { get; set; }
    }
}
