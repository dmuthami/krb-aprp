using Newtonsoft.Json;

namespace APRP.Web.ViewModels.COSTES
{
    public class GetInstructutedWorkItemsViewModel
    {
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("ItemCode")]
        public string ItemCode { get; set; }
        [JsonProperty("SubItemCode")]
        public string SubItemCode { get; set; }
        [JsonProperty("SubSubItemCode")]
        public string SubSubItemCode { get; set; }
        [JsonProperty("Unit")]
        public string Unit { get; set; }
        [JsonProperty("UnitDescription")]
        public string UnitDescription { get; set; }
        [JsonProperty("UnitPrice")]
        public string UnitPrice { get; set; }
        [JsonProperty("WorkItem")]
        public string WorkItem { get; set; }
        [JsonProperty("SubItem")]
        public string SubItem { get; set; }
        [JsonProperty("OtherDescription")]
        public string OtherDescription { get; set; }
        [JsonProperty("SurveyYear")]
        public string SurveyYear { get; set; }
        [JsonProperty("OverHeadRoutineMaintanance")]
        public string OverHeadRoutineMaintanance { get; set; }

    }
}
