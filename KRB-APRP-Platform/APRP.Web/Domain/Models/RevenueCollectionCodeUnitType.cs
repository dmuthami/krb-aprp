namespace APRP.Web.Domain.Models
{
    public class RevenueCollectionCodeUnitType
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public ICollection<RevenueCollectionCodeUnit> RevenueCollectionCodeUnits { get; set; }
    }
}