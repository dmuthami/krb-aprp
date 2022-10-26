namespace APRP.Web.Claims
{
    public static class Permission
    {
        /**
         *  First, we define permissions for each action grouped by a
         *  feature area.In this example, we are defining two
         *  feature areas with CRUD permissions.
         *  We are using constants because we will use these later 
         *  in attributes, which require constant expressions.
         */      
        public static class KENHA
        {
            
            public const string GIS_View = "Permissions.KENHA.GIS.View";
            public const string GIS_Add = "Permissions.KENHA.GIS.Add";
            public const string GIS_Change = "Permissions.KENHA.GIS.Change";
            public const string GIS_Delete = "Permissions.KENHA.GIS.Delete";
        }

        public static class KERRA
        {
            public const string GIS_View = "Permissions.KERRA.GIS.View";
            public const string GIS_Add = "Permissions.KERRA.GIS.Add";
            public const string GIS_Change = "Permissions.KERRA.GIS.Change";
            public const string GIS_Delete = "Permissions.KERRA.GIS.Delete";
        }

        public static class KURA
        {
            public const string GIS_View = "Permissions.KURA.GIS.View";
            public const string GIS_Add = "Permissions.KURA.GIS.Add";
            public const string GIS_Change = "Permissions.KURA.GIS.Change";
            public const string GIS_Delete = "Permissions.KURA.GIS.Delete";
        }

        public static class KWS
        {
            public const string GIS_View = "Permissions.KWS.GIS.View";
            public const string GIS_Add = "Permissions.KWS.GIS.Add";
            public const string GIS_Change = "Permissions.KWS.GIS.Change";
            public const string GIS_Delete = "Permissions.KWS.GIS.Delete";
        }

        public static class Counties
        {
            public const string GIS_View = "Permissions.Counties.GIS.View";
            public const string GIS_Add = "Permissions.Counties.GIS.Add";
            public const string GIS_Change = "Permissions.Counties.GIS.Change";
            public const string GIS_Delete = "Permissions.Counties.GIS.Delete";
        }

        public static class Administrator
        {
            public const string Role_View = "Permissions.Administrator.Role.View";
            public const string Role_Add = "Permissions.Administrator.Role.Add";
            public const string Role_Change = "Permissions.Administrator.Role.Change";
            public const string Role_Delete = "Permissions.Administrator.Role.Delete";
            public const string ActivateOrDeactivate = "Permissions.Administrator.ActivateOrDeactivate";
        }

        public static class GIS
        {
            public const string View = "Permissions.GIS.View";
        }
        public static class ARICS
        {
            public const string View = "Permissions.ARICS.View";
            public const string ConductARICS = "Permissions.ARICS.ConductARICS";
            public const string Change = "Permissions.ARICS.Change";
            public const string PreparerOrSubmitter = "Permissions.ARICS.PreparerOrSubmitter";
            public const string FirstReviewer = "Permissions.ARICS.FirstReviewer";
            public const string SecondReviewer = "Permissions.ARICS.SecondReviewer";
            public const string InternalApprover = "Permissions.ARICS.InternalApprover";
            public const string Approver = "Permissions.ARICS.Approver";
            public const string ARICSYear_View = "Permissions.ARICSYear.View";
            public const string ARICSYear_Add = "Permissions.ARICSYear.Add";
            public const string ARICSYear_Change = "Permissions.ARICSYear.Change";
            public const string ARICSYear_Delete = "Permissions.ARICSYear.Delete";
        }
            public static class Finance
        {
            public const string View = "Permissions.Finance.View";
            public const string Add = "Permissions.Finance.Add";
            public const string Change = "Permissions.Finance.Change";
            public const string Delete = "Permissions.Finance.Delete";
        }
        public static class FinancialYear
        {
            public const string View = "Permissions.FinancialYear.View";
            public const string Add = "Permissions.FinancialYear.Add";
            public const string Change = "Permissions.FinancialYear.Change";
            public const string Delete = "Permissions.FinancialYear.Delete";
        }
        public static class Allocation
        {
            public const string View = "Permissions.Allocation.View";
            public const string Add = "Permissions.Allocation.Add";
            public const string Change = "Permissions.Allocation.Change";
            public const string Delete = "Permissions.Allocation.Delete";
        }
        public static class AllocationCodeUnit
        {
            public const string View = "Permissions.AllocationCodeUnit.View";
            public const string Add = "Permissions.AllocationCodeUnit.Add";
            public const string Change = "Permissions.AllocationCodeUnit.Change";
            public const string Delete = "Permissions.AllocationCodeUnit.Delete";
        }
        public static class Disbursement
        {
            public const string View = "Permissions.Disbursement.View";
            public const string Add = "Permissions.Disbursement.Add";
            public const string Change = "Permissions.Disbursement.Change";
            public const string Delete = "Permissions.Disbursement.Delete";
        }

        public static class Release
        {
            public const string View = "Permissions.Release.View";
            public const string Add = "Permissions.Release.Add";
            public const string Change = "Permissions.Release.Change";
            public const string Delete = "Permissions.Release.Delete";
        }

        public static class CSAllocation
        {
            public const string View = "Permissions.CSAllocation.View";
            public const string Add = "Permissions.CSAllocation.Add";
            public const string Change = "Permissions.CSAllocation.Change";
            public const string Delete = "Permissions.CSAllocation.Delete";
        }
        public static class RevenueCollection
        {
            public const string View = "Permissions.RevenueCollection.View";
            public const string Add = "Permissions.RevenueCollection.Add";
            public const string Change = "Permissions.RevenueCollection.Change";
            public const string Delete = "Permissions.RevenueCollection.Delete";
            public const string SubmitBudget = "Permissions.RevenueCollection.SubmitBudget";
            public const string ApproveBudget = "Permissions.RevenueCollection.ApproveBudget";
        }
        public static class RevenueCollectionCodeUnit
        {
            public const string View = "Permissions.RevenueCollectionCodeUnit.View";
            public const string Add = "Permissions.RevenueCollectionCodeUnit.Add";
            public const string Change = "Permissions.RevenueCollectionCodeUnit.Change";
            public const string Delete = "Permissions.RevenueCollectionCodeUnit.Delete";
        }
        public static class RevenueCollectionCodeUnitType
        {
            public const string View = "Permissions.RevenueCollectionCodeUnitType.View";
            public const string Add = "Permissions.RevenueCollectionCodeUnitType.Add";
            public const string Change = "Permissions.RevenueCollectionCodeUnitType.Change";
            public const string Delete = "Permissions.RevenueCollectionCodeUnitType.Delete";
        }
        public static class WorkCategoryFundingMatrix
        {
            public const string View = "Permissions.WorkCategoryFundingMatrix.View";
            public const string Add = "Permissions.WorkCategoryFundingMatrix.Add";
            public const string Change = "Permissions.WorkCategoryFundingMatrix.Change";
            public const string Delete = "Permissions.WorkCategoryFundingMatrix.Delete";
        }

        public static class WorkCategoryAllocationMatrix
        {
            public const string View = "Permissions.WorkCategoryAllocationMatrix.View";
            public const string Add = "Permissions.WorkCategoryAllocationMatrix.Add";
            public const string Change = "Permissions.WorkCategoryAllocationMatrix.Change";
            public const string Delete = "Permissions.WorkCategoryAllocationMatrix.Delete";
        }
        public static class OtherFundItem
        {
            public const string View = "Permissions.OtherFundItem.View";
            public const string Add = "Permissions.OtherFundItem.Add";
            public const string Change = "Permissions.OtherFundItem.Change";
            public const string Delete = "Permissions.OtherFundItem.Delete";
        }
        public static class Training
        {
            public const string View = "Permissions.Training.View";
            public const string Add = "Permissions.Training.Add";
            public const string Change = "Permissions.Training.Change";
            public const string Delete = "Permissions.Training.Delete";
            public const string Report = "Permissions.Training.Report";
        }
        public static class TrainingCourse
        {
            public const string View = "Permissions.TrainingCourse.View";
            public const string Add = "Permissions.TrainingCourse.Add";
            public const string Change = "Permissions.TrainingCourse.Change";
            public const string Delete = "Permissions.TrainingCourse.Delete";
        }
        public static class QuarterCodeList
        {
            public const string View = "Permissions.QuarterCodeList.View";
            public const string Add = "Permissions.QuarterCodeList.Add";
            public const string Change = "Permissions.QuarterCodeList.Change";
            public const string Delete = "Permissions.QuarterCodeList.Delete";
        }

        public static class QuarterCodeUnit
        {
            public const string View = "Permissions.QuarterCodeUnit.View";
            public const string Add = "Permissions.QuarterCodeUnit.Add";
            public const string Change = "Permissions.QuarterCodeUnit.Change";
            public const string Delete = "Permissions.QuarterCodeUnit.Delete";
        }
        public static class RoadClassification
        {
            public const string View = "Permissions.RoadClassification.View";
            public const string Add = "Permissions.RoadClassification.Add";
            public const string Change = "Permissions.RoadClassification.Change";
            public const string Delete = "Permissions.RoadClassification.Delete";
            public const string Submit = "Permissions.RoadClassification.Submit";
            public const string Approve = "Permissions.RoadClassification.Approve";
            public const string AddRoadtoGIS = "Permissions.RoadClassification.AddRoadtoGIS";
        }

        public static class FundType
        {
            public const string View = "Permissions.FundType.View";
            public const string Add = "Permissions.FundType.Add";
            public const string Change = "Permissions.FundType.Change";
            public const string Delete = "Permissions.FundType.Delete";
        }

        public static class FundingSource
        {
            public const string View = "Permissions.FundingSource.View";
            public const string Add = "Permissions.FundingSource.Add";
            public const string Change = "Permissions.FundingSource.Change";
            public const string Delete = "Permissions.FundingSource.Delete";
        }

        public static class ActivityItem
        {
            public const string View = "Permissions.ActivityItem.View";
            public const string Add = "Permissions.ActivityItem.Add";
            public const string Change = "Permissions.ActivityItem.Change";
            public const string Delete = "Permissions.ActivityItem.Delete";
        }

        public static class ActivityGroup
        {
            public const string View = "Permissions.ActivityGroup.View";
            public const string Add = "Permissions.ActivityGroup.Add";
            public const string Change = "Permissions.ActivityGroup.Change";
            public const string Delete = "Permissions.ActivityGroup.Delete";
        }

        public static class Authority
        {
            public const string View = "Permissions.Authority.View";
            public const string Add = "Permissions.Authority.Add";
            public const string Change = "Permissions.Authority.Change";
            public const string Delete = "Permissions.Authority.Delete";
        }

        public static class Contractor
        {
            public const string View = "Permissions.Contractor.View";
            public const string Save = "Permissions.Contractor.Save";
            public const string Delete = "Permissions.Contractor.Delete";
        }

        public static class RoadClassCodeUnit
        {
            public const string View = "Permissions.RoadClassCodeUnit.View";
            public const string Add = "Permissions.RoadClassCodeUnit.Add";
            public const string Change = "Permissions.RoadClassCodeUnit.Change";
            public const string Delete = "Permissions.RoadClassCodeUnit.Delete";
        }

        public static class RoadConditionCodeUnit
        {
            public const string View = "Permissions.RoadConditionCodeUnit.View";
            public const string Add = "Permissions.RoadConditionCodeUnit.Add";
            public const string Change = "Permissions.RoadConditionCodeUnit.Change";
            public const string Delete = "Permissions.RoadConditionCodeUnit.Delete";
        }

        public static class RoadPrioritization
        {
            public const string View = "Permissions.RoadPrioritization.View";
            public const string Add = "Permissions.RoadPrioritization.Add";
            public const string Change = "Permissions.RoadPrioritization.Change";
            public const string Delete = "Permissions.RoadPrioritization.Delete";
        }

        public static class BudgetCeiling
        {
            public const string Upload = "Permissions.BudgetCeiling.Upload";
            public const string View = "Permissions.BudgetCeiling.View";
            public const string Add = "Permissions.BudgetCeiling.Add";
            public const string Change = "Permissions.BudgetCeiling.Change";
            public const string Delete = "Permissions.BudgetCeiling.Delete";
            public const string Download = "Permissions.BudgetCeiling.Download";
            public const string DeleteLetter = "Permissions.BudgetCeiling.DeleteLetter";
            public const string Report = "Permissions.BudgetCeiling.Report";
        }

        public static class BudgetCeilingComputation
        {
            public const string Upload = "Permissions.BudgetCeilingComputation.Upload";
            public const string View = "Permissions.BudgetCeilingComputation.View";
            public const string Add = "Permissions.BudgetCeilingComputation.Add";
            public const string Change = "Permissions.BudgetCeilingComputation.Change";
            public const string Delete = "Permissions.BudgetCeilingComputation.Delete";
            public const string Download = "Permissions.BudgetCeilingComputation.Download";
            public const string Report = "Permissions.BudgetCeilingComputation.Report";
        }

        //workplan permissions
        public static class WorkplanPermissions
        {
            public const string Upload = "Permissions.WorkplanPermissions.Upload";
            public const string Approver1 = "Permissions.WorkplanPermissions.Approver1";
            public const string Approver2 = "Permissions.WorkplanPermissions.Approver2";
            public const string Approver3 = "Permissions.WorkplanPermissions.Approver3";
            public const string Approver4 = "Permissions.WorkplanPermissions.Approver4";
            public const string Approver5 = "Permissions.WorkplanPermissions.Approver5";
            public const string Approver6 = "Permissions.WorkplanPermissions.Approver6";
            public const string Add = "Permissions.WorkplanPermissions.Add";
            public const string View = "Permissions.WorkplanPermissions.View";
            public const string Edit = "Permissions.WorkplanPermissions.Edit";
            public const string Delete = "Permissions.WorkplanPermissions.Delete";
        }

        //Costes permissions
        public static class COSTES
        {
            public const string View = "Permissions.COSTES.View";
            public const string Add = "Permissions.COSTES.Add";
            public const string Change = "Permissions.COSTES.Change";
            public const string Delete = "Permissions.COSTES.Delete";
        }
        public static class Road
        {
            public const string View = "Permissions.Road.View";
            public const string Add = "Permissions.Road.Add";
            public const string Change = "Permissions.Road.Change";
            public const string Delete = "Permissions.Road.Delete";
        }

        public static class RoadSection
        {
            public const string View = "Permissions.RoadSection.View";
            public const string Add = "Permissions.RoadSection.Add";
            public const string Change = "Permissions.RoadSection.Change";
            public const string Delete = "Permissions.RoadSection.Delete";
        }
    }
}
