namespace APRP.Web.Claims
{
    public static class AllPermissions
    {
        public static Dictionary<string, string> GlobalPermissions = new Dictionary<string, string>();
        static AllPermissions()
        {
            GlobalPermissions.Add("Permissions.KENHA.GIS.View", "Permissions.KENHA.GIS.View");
            GlobalPermissions.Add("Permissions.KENHA.GIS.Add", "Permissions.KENHA.GIS.Add");
            GlobalPermissions.Add("Permissions.KENHA.GIS.Change", "Permissions.KENHA.GIS.Change");
            GlobalPermissions.Add("Permissions.KENHA.GIS.Delete", "Permissions.KENHA.GIS.Delete");

            GlobalPermissions.Add("Permissions.KERRA.GIS.View", "Permissions.KERRA.GIS.View");
            GlobalPermissions.Add("Permissions.KERRA.GIS.Add", "Permissions.KERRA.GIS.Add");
            GlobalPermissions.Add("Permissions.KERRA.GIS.Change", "Permissions.KERRA.GIS.Change");
            GlobalPermissions.Add("Permissions.KERRA.GIS.Delete", "Permissions.KERRA.GIS.Delete");

            GlobalPermissions.Add("Permissions.KURA.GIS.View", "Permissions.KURA.GIS.View");
            GlobalPermissions.Add("Permissions.KURA.GIS.Add", "Permissions.KURA.GIS.Add");
            GlobalPermissions.Add("Permissions.KURA.GIS.Change", "Permissions.KURA.GIS.Change");
            GlobalPermissions.Add("Permissions.KURA.GIS.Delete", "Permissions.KURA.GIS.Delete");

            GlobalPermissions.Add("Permissions.KWS.GIS.View", "Permissions.KWS.GIS.View");
            GlobalPermissions.Add("Permissions.KWS.GIS.Add", "Permissions.KWS.GIS.Add");
            GlobalPermissions.Add("Permissions.KWS.GIS.Change", "Permissions.KWS.GIS.Change");
            GlobalPermissions.Add("Permissions.KWS.GIS.Delete", "Permissions.KWS.GIS.Delete");

            GlobalPermissions.Add("Permissions.Counties.GIS.View", "Permissions.Counties.GIS.View");
            GlobalPermissions.Add("Permissions.Counties.GIS.Add", "Permissions.Counties.GIS.Add");
            GlobalPermissions.Add("Permissions.Counties.GIS.Change", "Permissions.Counties.GIS.Change");
            GlobalPermissions.Add("Permissions.Counties.GIS.Delete", "Permissions.Counties.GIS.Delete");

            GlobalPermissions.Add("Permissions.Administrator.Role.View", "Permissions.Administrator.Role.View");
            GlobalPermissions.Add("Permissions.Administrator.Role.Add", "Permissions.Administrator.Role.Add");
            GlobalPermissions.Add("Permissions.Administrator.Role.Change", "Permissions.Administrator.Role.Change");
            GlobalPermissions.Add("Permissions.Administrator.Role.Delete", "Permissions.Administrator.Role.Delete");
            GlobalPermissions.Add("Permissions.Administrator.ActivateOrDeactivate", "Permissions.Administrator.ActivateOrDeactivate");

            GlobalPermissions.Add("Permissions.GIS.View", "Permissions.GIS.View");

            GlobalPermissions.Add("Permissions.ARICS.View", "Permissions.ARICS.View");
            GlobalPermissions.Add("Permissions.ARICS.ConductARICS", "Permissions.ARICS.ConductARICS");
            GlobalPermissions.Add("Permissions.ARICS.Change", "Permissions.ARICS.Change");
            GlobalPermissions.Add("Permissions.ARICS.PreparerOrSubmitter", "Permissions.ARICS.PreparerOrSubmitter");
            GlobalPermissions.Add("Permissions.ARICS.FirstReviewer", "Permissions.ARICS.FirstReviewer");
            GlobalPermissions.Add("Permissions.ARICS.SecondReviewer", "Permissions.ARICS.SecondReviewer");
            GlobalPermissions.Add("Permissions.ARICS.InternalApprover", "Permissions.ARICS.InternalApprover");
            GlobalPermissions.Add("Permissions.ARICS.Approver", "Permissions.ARICS.Approver");
            GlobalPermissions.Add("Permissions.ARICSYear.View", "Permissions.ARICSYear.View");
            GlobalPermissions.Add("Permissions.ARICSYear.Add", "Permissions.ARICSYear.Add");
            GlobalPermissions.Add("Permissions.ARICSYear.Change", "Permissions.ARICSYear.Change");
            GlobalPermissions.Add("Permissions.ARICSYear.Delete", "Permissions.ARICSYear.Delete");

            GlobalPermissions.Add("Permissions.Finance.View", "Permissions.Finance.View");
            GlobalPermissions.Add("Permissions.Finance.Add", "Permissions.Finance.Add");
            GlobalPermissions.Add("Permissions.Finance.Change", "Permissions.Finance.Change");
            GlobalPermissions.Add("Permissions.Finance.Delete", "Permissions.Finance.Delete");

            GlobalPermissions.Add("Permissions.FinancialYear.View", "Permissions.FinancialYear.View");
            GlobalPermissions.Add("Permissions.FinancialYear.Add", "Permissions.FinancialYear.Add");
            GlobalPermissions.Add("Permissions.FinancialYear.Change", "Permissions.FinancialYear.Change");
            GlobalPermissions.Add("Permissions.FinancialYear.Delete", "Permissions.FinancialYear.Delete");

            GlobalPermissions.Add("Permissions.Allocation.View", "Permissions.Allocation.View");
            GlobalPermissions.Add("Permissions.Allocation.Add", "Permissions.Allocation.Add");
            GlobalPermissions.Add("Permissions.Allocation.Change", "Permissions.Allocation.Change");
            GlobalPermissions.Add("Permissions.Allocation.Delete", "Permissions.Allocation.Delete");

            GlobalPermissions.Add("Permissions.AllocationCodeUnit.View", "Permissions.AllocationCodeUnit.View");
            GlobalPermissions.Add("Permissions.AllocationCodeUnit.Add", "Permissions.AllocationCodeUnit.Add");
            GlobalPermissions.Add("Permissions.AllocationCodeUnit.Change", "Permissions.AllocationCodeUnit.Change");
            GlobalPermissions.Add("Permissions.AllocationCodeUnit.Delete", "Permissions.AllocationCodeUnit.Delete");

            GlobalPermissions.Add("Permissions.Disbursement.View", "Permissions.Disbursement.View");
            GlobalPermissions.Add("Permissions.Disbursement.Add", "Permissions.Disbursement.Add");
            GlobalPermissions.Add("Permissions.Disbursement.Change", "Permissions.Disbursement.Change");
            GlobalPermissions.Add("Permissions.Disbursement.Delete", "Permissions.Disbursement.Delete");

            GlobalPermissions.Add("Permissions.Release.View", "Permissions.Release.View");
            GlobalPermissions.Add("Permissions.Release.Add", "Permissions.Release.Add");
            GlobalPermissions.Add("Permissions.Release.Change", "Permissions.Release.Change");
            GlobalPermissions.Add("Permissions.Release.Delete", "Permissions.Release.Delete");

            GlobalPermissions.Add("Permissions.CSAllocation.View", "Permissions.CSAllocation.View");
            GlobalPermissions.Add("Permissions.CSAllocation.Add", "Permissions.CSAllocation.Add");
            GlobalPermissions.Add("Permissions.CSAllocation.Change", "Permissions.CSAllocation.Change");
            GlobalPermissions.Add("Permissions.CSAllocation.Delete", "Permissions.CSAllocation.Delete");

            GlobalPermissions.Add("Permissions.RevenueCollection.View", "Permissions.RevenueCollection.View");
            GlobalPermissions.Add("Permissions.RevenueCollection.Add", "Permissions.RevenueCollection.Add");
            GlobalPermissions.Add("Permissions.RevenueCollection.Change", "Permissions.RevenueCollection.Change");
            GlobalPermissions.Add("Permissions.RevenueCollection.Delete", "Permissions.RevenueCollection.Delete");
            GlobalPermissions.Add("Permissions.RevenueCollection.SubmitBudget", "Permissions.RevenueCollection.SubmitBudget");
            GlobalPermissions.Add("Permissions.RevenueCollection.ApproveBudget", "Permissions.RevenueCollection.ApproveBudget");

            GlobalPermissions.Add("Permissions.RevenueCollectionCodeUnit.View", "Permissions.RevenueCollectionCodeUnit.View");
            GlobalPermissions.Add("Permissions.RevenueCollectionCodeUnit.Add", "Permissions.RevenueCollectionCodeUnit.Add");
            GlobalPermissions.Add("Permissions.RevenueCollectionCodeUnit.Change", "Permissions.RevenueCollectionCodeUnit.Change");
            GlobalPermissions.Add("Permissions.RevenueCollectionCodeUnit.Delete", "Permissions.RevenueCollectionCodeUnit.Delete");

            GlobalPermissions.Add("Permissions.RevenueCollectionCodeUnitType.View", "Permissions.RevenueCollectionCodeUnitType.View");
            GlobalPermissions.Add("Permissions.RevenueCollectionCodeUnitType.Add", "Permissions.RevenueCollectionCodeUnitType.Add");
            GlobalPermissions.Add("Permissions.RevenueCollectionCodeUnitType.Change", "Permissions.RevenueCollectionCodeUnitType.Change");
            GlobalPermissions.Add("Permissions.RevenueCollectionCodeUnitType.Delete", "Permissions.RevenueCollectionCodeUnitType.Delete");

            GlobalPermissions.Add("Permissions.WorkCategoryFundingMatrix.View", "Permissions.WorkCategoryFundingMatrix.View");
            GlobalPermissions.Add("Permissions.WorkCategoryFundingMatrix.Add", "Permissions.WorkCategoryFundingMatrix.Add");
            GlobalPermissions.Add("Permissions.WorkCategoryFundingMatrix.Change", "Permissions.WorkCategoryFundingMatrix.Change");
            GlobalPermissions.Add("Permissions.WorkCategoryFundingMatrix.Delete", "Permissions.WorkCategoryFundingMatrix.Delete");

            GlobalPermissions.Add("Permissions.WorkCategoryAllocationMatrix.View", "Permissions.WorkCategoryAllocationMatrix.View");
            GlobalPermissions.Add("Permissions.WorkCategoryAllocationMatrix.Add", "Permissions.WorkCategoryAllocationMatrix.Add");
            GlobalPermissions.Add("Permissions.WorkCategoryAllocationMatrix.Change", "Permissions.WorkCategoryAllocationMatrix.Change");
            GlobalPermissions.Add("Permissions.WorkCategoryAllocationMatrix.Delete", "Permissions.WorkCategoryAllocationMatrix.Delete");

            GlobalPermissions.Add("Permissions.OtherFundItem.View", "Permissions.OtherFundItem.View");
            GlobalPermissions.Add("Permissions.OtherFundItem.Add", "Permissions.OtherFundItem.Add");
            GlobalPermissions.Add("Permissions.OtherFundItem.Change", "Permissions.OtherFundItem.Change");
            GlobalPermissions.Add("Permissions.OtherFundItem.Delete", "Permissions.OtherFundItem.Delete");

            GlobalPermissions.Add("Permissions.Training.View", "Permissions.Training.View");
            GlobalPermissions.Add("Permissions.Training.Add", "Permissions.Training.Add");
            GlobalPermissions.Add("Permissions.Training.Change", "Permissions.Training.Change");
            GlobalPermissions.Add("Permissions.Training.Delete", "Permissions.Training.Delete");
            GlobalPermissions.Add("Permissions.Training.Report", "Permissions.Training.Report");

            GlobalPermissions.Add("Permissions.TrainingCourse.View", "Permissions.TrainingCourse.View");
            GlobalPermissions.Add("Permissions.TrainingCourse.Add", "Permissions.TrainingCourse.Add");
            GlobalPermissions.Add("Permissions.TrainingCourse.Change", "Permissions.TrainingCourse.Change");
            GlobalPermissions.Add("Permissions.TrainingCourse.Delete", "Permissions.TrainingCourse.Delete");

            GlobalPermissions.Add("Permissions.QuarterCodeList.View", "Permissions.QuarterCodeList.View");
            GlobalPermissions.Add("Permissions.QuarterCodeList.Add", "Permissions.QuarterCodeList.Add");
            GlobalPermissions.Add("Permissions.QuarterCodeList.Change", "Permissions.QuarterCodeList.Change");
            GlobalPermissions.Add("Permissions.QuarterCodeList.Delete", "Permissions.QuarterCodeList.Delete");

            GlobalPermissions.Add("Permissions.QuarterCodeUnit.View", "Permissions.QuarterCodeUnit.View");
            GlobalPermissions.Add("Permissions.QuarterCodeUnit.Add", "Permissions.QuarterCodeUnit.Add");
            GlobalPermissions.Add("Permissions.QuarterCodeUnit.Change", "Permissions.QuarterCodeUnit.Change");
            GlobalPermissions.Add("Permissions.QuarterCodeUnit.Delete", "Permissions.QuarterCodeUnit.Delete");

            GlobalPermissions.Add("Permissions.RoadClassification.View", "Permissions.RoadClassification.View");
            GlobalPermissions.Add("Permissions.RoadClassification.Add", "Permissions.RoadClassification.Add");
            GlobalPermissions.Add("Permissions.RoadClassification.Change", "Permissions.RoadClassification.Change");
            GlobalPermissions.Add("Permissions.RoadClassification.Delete", "Permissions.RoadClassification.Delete");
            GlobalPermissions.Add("Permissions.RoadClassification.Submit", "Permissions.RoadClassification.Submit");
            GlobalPermissions.Add("Permissions.RoadClassification.Approve", "Permissions.RoadClassification.Approve");
            GlobalPermissions.Add("Permissions.RoadClassification.AddRoadtoGIS", "Permissions.RoadClassification.AddRoadtoGIS");

            GlobalPermissions.Add("Permissions.FundType.View", "Permissions.FundType.View");
            GlobalPermissions.Add("Permissions.FundType.Add", "Permissions.FundType.Add");
            GlobalPermissions.Add("Permissions.FundType.Change", "Permissions.FundType.Change");
            GlobalPermissions.Add("Permissions.FundType.Delete", "Permissions.FundType.Delete");

            GlobalPermissions.Add("Permissions.FundingSource.View", "Permissions.FundingSource.View");
            GlobalPermissions.Add("Permissions.FundingSource.Add", "Permissions.FundingSource.Add");
            GlobalPermissions.Add("Permissions.FundingSource.Change", "Permissions.FundingSource.Change");
            GlobalPermissions.Add("Permissions.FundingSource.Delete", "Permissions.FundingSource.Delete");

            GlobalPermissions.Add("Permissions.ActivityItem.View", "Permissions.ActivityItem.View");
            GlobalPermissions.Add("Permissions.ActivityItem.Add", "Permissions.ActivityItem.Add");
            GlobalPermissions.Add("Permissions.ActivityItem.Change", "Permissions.ActivityItem.Change");
            GlobalPermissions.Add("Permissions.ActivityItem.Delete", "Permissions.ActivityItem.Delete");

            GlobalPermissions.Add("Permissions.ActivityGroup.View", "Permissions.ActivityGroup.View");
            GlobalPermissions.Add("Permissions.ActivityGroup.Add", "Permissions.ActivityGroup.Add");
            GlobalPermissions.Add("Permissions.ActivityGroup.Change", "Permissions.ActivityGroup.Change");
            GlobalPermissions.Add("Permissions.ActivityGroup.Delete", "Permissions.ActivityGroup.Delete");

            GlobalPermissions.Add("Permissions.Authority.View", "Permissions.Authority.View");
            GlobalPermissions.Add("Permissions.Authority.Add", "Permissions.Authority.Add");
            GlobalPermissions.Add("Permissions.Authority.Change", "Permissions.Authority.Change");
            GlobalPermissions.Add("Permissions.Authority.Delete", "Permissions.Authority.Delete");

            GlobalPermissions.Add("Permissions.Contractor.View", "Permissions.Contractor.View");
            GlobalPermissions.Add("Permissions.Contractor.Save", "Permissions.Contractor.Save");
            GlobalPermissions.Add("Permissions.Contractor.Delete", "Permissions.Contractor.Delete");

            GlobalPermissions.Add("Permissions.RoadClassCodeUnit.View", "Permissions.RoadClassCodeUnit.View");
            GlobalPermissions.Add("Permissions.RoadClassCodeUnit.Add", "Permissions.RoadClassCodeUnit.Add");
            GlobalPermissions.Add("Permissions.RoadClassCodeUnit.Change", "Permissions.RoadClassCodeUnit.Change");
            GlobalPermissions.Add("Permissions.RoadClassCodeUnit.Delete", "Permissions.RoadClassCodeUnit.Delete");

            GlobalPermissions.Add("Permissions.RoadConditionCodeUnit.View", "Permissions.RoadConditionCodeUnit.View");
            GlobalPermissions.Add("Permissions.RoadConditionCodeUnit.Add", "Permissions.RoadConditionCodeUnit.Add");
            GlobalPermissions.Add("Permissions.RoadConditionCodeUnit.Change", "Permissions.RoadConditionCodeUnit.Change");
            GlobalPermissions.Add("Permissions.RoadConditionCodeUnit.Delete", "Permissions.RoadConditionCodeUnit.Delete");

            GlobalPermissions.Add("Permissions.RoadPrioritization.View", "Permissions.RoadPrioritization.View");
            GlobalPermissions.Add("Permissions.RoadPrioritization.Add", "Permissions.RoadPrioritization.Add");
            GlobalPermissions.Add("Permissions.RoadPrioritization.Change", "Permissions.RoadPrioritization.Change");
            GlobalPermissions.Add("Permissions.RoadPrioritization.Delete", "Permissions.RoadPrioritization.Delete");

            GlobalPermissions.Add("Permissions.BudgetCeiling.View", "Permissions.BudgetCeiling.View");
            GlobalPermissions.Add("Permissions.BudgetCeiling.Add", "Permissions.BudgetCeiling.Add");
            GlobalPermissions.Add("Permissions.BudgetCeiling.Change", "Permissions.BudgetCeiling.Change");
            GlobalPermissions.Add("Permissions.BudgetCeiling.Delete", "Permissions.BudgetCeiling.Delete");
            GlobalPermissions.Add("Permissions.BudgetCeiling.Upload", "Permissions.BudgetCeiling.Upload");
            GlobalPermissions.Add("Permissions.BudgetCeiling.Download", "Permissions.BudgetCeiling.Download");
            GlobalPermissions.Add("Permissions.BudgetCeiling.DeleteLetter", "Permissions.BudgetCeiling.DeleteLetter");
            GlobalPermissions.Add("Permissions.BudgetCeiling.Report", "Permissions.BudgetCeiling.Report");

            GlobalPermissions.Add("Permissions.BudgetCeilingComputation.Add", "Permissions.BudgetCeilingComputation.Add");
            GlobalPermissions.Add("Permissions.BudgetCeilingComputation.Change", "Permissions.BudgetCeilingComputation.Change");
            GlobalPermissions.Add("Permissions.BudgetCeilingComputation.Delete", "Permissions.BudgetCeilingComputation.Delete");
            GlobalPermissions.Add("Permissions.BudgetCeilingComputation.Upload", "Permissions.BudgetCeilingComputation.Upload");
            GlobalPermissions.Add("Permissions.BudgetCeilingComputation.Download", "Permissions.BudgetCeilingComputation.Download");
            GlobalPermissions.Add("Permissions.BudgetCeilingComputation.Report", "Permissions.BudgetCeilingComputation.Report");

            //workplan permissions

            GlobalPermissions.Add("Permissions.WorkplanPermissions.Upload", "Permissions.WorkplanPermissions.Upload");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Approver1", "Permissions.WorkplanPermissions.Approver1");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Approver2", "Permissions.WorkplanPermissions.Approver2");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Approver3", "Permissions.WorkplanPermissions.Approver3");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Approver4", "Permissions.WorkplanPermissions.Approver4");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Approver5", "Permissions.WorkplanPermissions.Approver5");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Approver6", "Permissions.WorkplanPermissions.Approver6");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Add", "Permissions.WorkplanPermissions.Add");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.View", "Permissions.WorkplanPermissions.View");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Edit", "Permissions.WorkplanPermissions.Edit");
            GlobalPermissions.Add("Permissions.WorkplanPermissions.Delete", "Permissions.WorkplanPermissions.Delete");

            //Costes permissions
            GlobalPermissions.Add("Permissions.COSTES.View", "Permissions.COSTES.View");
            GlobalPermissions.Add("Permissions.COSTES.Add", "Permissions.COSTES.Add");
            GlobalPermissions.Add("Permissions.COSTES.Change", "Permissions.COSTES.Change");
            GlobalPermissions.Add("Permissions.COSTES.Delete", "Permissions.COSTES.Delete");

            //Road permissions
            GlobalPermissions.Add("Permissions.Road.View", "Permissions.Road.View");
            GlobalPermissions.Add("Permissions.Road.Add", "Permissions.Road.Add");
            GlobalPermissions.Add("Permissions.Road.Change", "Permissions.Road.Change");
            GlobalPermissions.Add("Permissions.Road.Delete", "Permissions.Road.Delete");

            //Road Section permissions
            GlobalPermissions.Add("Permissions.RoadSection.View", "Permissions.RoadSection.View");
            GlobalPermissions.Add("Permissions.RoadSection.Add", "Permissions.RoadSection.Add");
            GlobalPermissions.Add("Permissions.RoadSection.Change", "Permissions.RoadSection.Change");
            GlobalPermissions.Add("Permissions.RoadSection.Delete", "Permissions.RoadSection.Delete");
        }
    }
}
