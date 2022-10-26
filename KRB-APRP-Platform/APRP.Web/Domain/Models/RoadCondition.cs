using System.ComponentModel;

namespace APRP.Web.Domain.Models
{
    public class RoadCondition
    {
        public long ID { get; set; }

        public double ARD { get; set; }

        public double IRI { get; set; }

        public DateTime ComputationTime { get; set; }

        public int Year { get; set; }

        [DisplayName("Priority Rate")]
        public long PriorityRate { get; set; }

        public string Comment { get; set; }

        /*----Navigation Properties--------------------------*/
        public long RoadId { get; set; }
        public virtual Road Road { get; set; }

        //public int RoadPrioritizationId { get; set; }
        //public RoadPrioritization RoadPrioritization { get; set; }

        /*----Navigation Properties--------------------------*/

    }
}
