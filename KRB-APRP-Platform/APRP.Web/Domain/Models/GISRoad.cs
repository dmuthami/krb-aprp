using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APRP.Web.Domain.Models
{
    public class GISRoad
    {
        //[Column("id")]
        public long ID { get; set; }

        [Column("section_id")]
        public string SectionID { get; set; }

        [Column("length")]
        public Decimal Length { get; set; }

        [Column(TypeName = "geometry")]
        public MultiLineString Geom { get; set; }

        [Column("cw_surf_co")]
        public string CWSurfaceCondition { get; set; }

        [Column("cw_surf_ty")]
        //[StringLength(10)]
        public string CWSurfaceType { get; set; }

        [Column("cwwid")]
        public int CarriageWidth { get; set; }

        [Column("cylane")]
        public string CycleLane { get; set; }

        [StringLength(15)]
        [Column("direction")]
        public string Direction { get; set; }

        [Column("draincond")]
        public string DrainageCondition { get; set; }

        [Column("draintype")]
        public string DrainType { get; set; }

        [Column("fpath")]
        public string FootPath { get; set; }

        [Column("inventdate")]
        //[Column(TypeName = "Date")]
        public string InventoryDate { get; set; }

        [Column("iri")]
        public Nullable<Decimal> InternationalRoughnessIndex { get; set; }

        [Column("loccylane")]
        public string LocationOfCycleLane { get; set; }

        [Column("locdrain")]
        public string LocationOfDrain { get; set; }

        [Column("locfpath")]
        public string LocalFootPath { get; set; }

        [Column("median")]
        public string RoadMedian { get; set; }

        [Column("mediantyp")]
        public string MedianType { get; set; }

        [Column("numlanes")]
        public Nullable<int> NumberOfLanes { get; set; }

        [Column("pci")]
        public Nullable<Decimal> PCI { get; set; }

        [Column("rdclass")]
        [StringLength(3)]
        //[Required]
        public string RoadClass { get; set; }

        [Column("rdname")]
        [StringLength(70)]
        public string RoadName { get; set; }

        [Column("rdnum")]
        public string RoadNumber { get; set; }

        [Column("rdreserve")]
        public int RoadReserve { get; set; }

        [Column("rdtype")]
        [StringLength(30)]
        //[Required]
        public string RoadType { get; set; }

        [Column("rdwid")]
        public int RoadWid { get; set; }

        [Column("roadusage")]
        [StringLength(10)]
        public string RoadUsage { get; set; }

        [Column("shouldcond")]
        public string ShoulderCondition { get; set; }

        [Column("shoulder")]
        public string Shoulder { get; set; }

        [Column("shouldmat")]
        public string ShoulderSurfaceType { get; set; }

        [Column("shouldwid")]
        public int ShoulderWidth { get; set; }

        [Column("sn")]
        public Nullable<Decimal> SN { get; set; }

        [Column("speedlt")]
        public int SpeedLimit { get; set; }

        [Column("stlight")]
        public string StreetLight { get; set; }

        [Column("surface_ty")]
        public string SurfaceType { get; set; }

        [Column("surveyor")]
        public string Surveyor { get; set; }

        /*-------Navigation Property---------*/
        public long RoadId { get; set; }
        public Road Road { get; set; }
        /*-------Navigation Property---------*/
    }
}
