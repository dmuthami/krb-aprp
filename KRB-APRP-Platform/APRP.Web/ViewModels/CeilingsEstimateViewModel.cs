namespace APRP.Web.ViewModels
{
    public class CeilingsEstimateViewModel
    {
        public double TotalFundsAvailable { get; set; } = 0d; //Code=1
        public double FuelLevy { get; set; } //code=2

        public double TransitTolls { get; set; } //code=3

        public double KRBOperations { get; set; } //code=4

        //KRB: 2% Transit Tolls
        public double KRBTransitTrolls { get; set; } //code=5

        public double KRBTotalOperations { get; set; } //code=6

        public double NETFundsAvailableAfterAdministrationCosts { get; set; } //code=7

        public double Road_Agencies_Fuel_Levy { get; set; } //code=8

        public double Road_Annuity_Fund { get; set; } //code=9

        public double SubTotal_Fuel_Levy { get; set; } //code=10

        public double NetTransitTolls { get; set; } //code=11

        public double TOTALFundsForAllocation { get; set; } //code=12

        public double RMLFLessRAF { get; set; } //code=13

        public double CountyAllocation { get; set; } //code=14

        //----Kenha-----
        public double kenhaAllocation { get; set; }  //code=15
        public double kenhaTransitTolls { get; set; } //code=16
        public double kenhaTotalAllocation { get; set; } //code=17
        public double kenhaAdminOperations { get; set; } //code=18
        public double kenhaRoadWorks { get; set; } //code=19
        //----End Kenha-----

        public double KRBBoard_CSAllocation { get; set; } //code=20

        public double KWSAllocation { get; set; } //code=21

        //----Kura-----
        public double KURAAllocation { get; set; } //code=22
        public double KURAAdminOperations { get; set; } //code=23

        public double KURARoadWorks { get; set; } //code=24
        //----End KURA-----

        public double kenhaTotal { get; set; } //code=25

        public double SubtotalFundsAllocated { get; set; } //code=26
        public double GrossRMLF { get; set; } //code=27

        //----Kerra-----
        public double KERRAAdminOperations { get; set; } //code=28
        public double KERRAAllocation { get; set; } //code=30
        public double KERRAConstituencyAllocation { get; set; }//code=31
        public double KERRACriticalLinksAllocation { get; set; }//code=32
        public double KERRATotalAdminBudget{ get; set; }//code=33
        public double KERRAPortinAdminTwentyTwoPercent { get; set; }//code=34
        public double KERRAPortinAdminTenPercent{ get; set; }//code=35
        public double KERRAPortinRoadWorksTwentyTwoPercent { get; set; }//code=36
        public double KERRAPortinRoadworksTenPercent { get; set; }//code=37
        public double KERRATotalBudgetForWorks { get; set; }//code=38
        public double KERRATwentyTwoPercentAllocPerConstituency { get; set; }//code=39
        public double KERRATenPercentAllocPerConstituency { get; set; }//code=40
        public double KERRATotalAllocPerConstituency { get; set; }//code=41
        public double NationalRoadsDevelopment { get; set; }//code=42: Kenha
        //----End Kerra-----
        public double NationalRoadsDevelopmentKeRRA { get; set; }//code=43: Kerra
        public double NationalRoadsDevelopmentKurA { get; set; }//code=44: Kura
        public double NationalRoadsDevelopmentKwS { get; set; }//code=45: KwS
        


    }
}
