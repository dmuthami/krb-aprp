using System.ComponentModel.DataAnnotations;

namespace APRP.Web.Domain.Models.Abstract
{
    public class RoadClassificationAbstract
    {
        /*
         * RA/County name
         */
        [Display(Name = "Authority")]
        public long AuthorityId { get; set; }

        [Display(Name = "Authority")]
        public Authority Authority { get; set; }

        [Display(Name = "Road ID")]
        public string RoadId { get; set; }

        [Display(Name = "Road Name")]
        public string RoadName { get; set; }

        [Display(Name = "Start Point")]
        public string StartPoint { get; set; }

        [Display(Name = "End Point")]
        public string EndPont { get; set; }

        [Display(Name = "Total Road Length")]
        public double TotalRoadLength { get; set; }

        [Display(Name = "Traverses More Than One County")]
        public bool TraversesMoreThanOneCounty { get; set; }

        [Display(Name = "List County Names")]
        public string ListCountyNames { get; set; }

        [Display(Name = "Link County HQ To Another")]
        public bool LinkCountyHQtoAnother { get; set; }

        [Display(Name = "Link County HQ Names")]
        public string ListCountyHQNames { get; set; }

        [Display(Name = "Link Sub County To Another")]
        public bool LinkSubCountytoAnother { get; set; }

        [Display(Name = "Link Sub County Names")]
        public string ListSubCountyNames { get; set; }

        [Display(Name = "Access To Public Facility")]
        public bool AccessToPublicFacility { get; set; }

        [Display(Name = "List Public Facility")]
        public string ListPublicFacility { get; set; }

        //Does the road provide access to a Government office?
        [Display(Name = "Access To Government Office")]
        public bool AccessToGovernmentOffice { get; set; }
        [Display(Name = "List Government Office")]
        public string ListGovernmentOffice { get; set; }

        [Display(Name = "Carriageway Width In Meters")]
        public int CwayWidthInMeters { get; set; }

        [Display(Name = "Road Reserve Size In Meters")]
        public int RoadReserveSizeInMeters { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }//UserId

        [Display(Name = "Update By")]
        public string UpdateBy { get; set; }

        [Display(Name = "Update Date")]
        public DateTime UpdateDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }
    }
}
