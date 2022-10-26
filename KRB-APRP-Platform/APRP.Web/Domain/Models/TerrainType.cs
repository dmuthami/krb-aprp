using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class TerrainType
    {
        public long ID { get; set; }

        [MaxLength(3)]
        public string Code { get; set; }

        [MaxLength(10)]
        public string Name { get; set; }

        public string Description { get; set; }

        //--Navigation
        /// <summary>
        /// Road sheet includes many terrain Types entity for the specified road sheet
        /// </summary>
        public ICollection<RoadSheet> RoadSheets { get; set; }
        //--Navigation
    }
}
