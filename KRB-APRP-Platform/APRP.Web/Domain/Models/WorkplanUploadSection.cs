using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models
{
    public class WorkplanUploadSection
    {
        [Key]
        public long ID { get; set; }
        public long WorkplanUploadId { get; set; }
        public WorkplanUpload WorkplanUpload { get; set; }

        public string  RegionCounty { get; set; }
        public string  Constituency { get; set; }
        public string  CityMunicipality { get; set; }
        public string  NationalPark { get; set; }
        public string RoadId { get; set; }
        public string SectionId { get; set; }
        public string SectionName { get; set; }
        public string SurfaceType { get; set; }
        public string WorkCategory { get; set; }
        public double PlannedSectionLength{ get; set; }
        public List<WorkplanUploadSectionActivity> WorkplanUploadSectionActivities{ get; set; }

    }
}
