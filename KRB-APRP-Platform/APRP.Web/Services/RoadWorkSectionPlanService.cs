using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Services
{
    public class RoadWorkSectionPlanService : IRoadWorkSectionPlanService
    {
        private readonly IRoadWorkSectionPlanRepository _roadWorkSectionPlanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RoadWorkSectionPlanService(IRoadWorkSectionPlanRepository roadWorkSectionPlanRepository, IUnitOfWork unitOfWork, ILogger<RoadWorkSectionPlanService> logger)
        {
            _roadWorkSectionPlanRepository = roadWorkSectionPlanRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkSectionPlanResponse> AddAsync(RoadWorkSectionPlan roadWorkSectionPlan)
        {
            try
            {
                await _roadWorkSectionPlanRepository.AddAsync(roadWorkSectionPlan).ConfigureAwait(false);
                await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                return new RoadWorkSectionPlanResponse(roadWorkSectionPlan);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkSectionPlanService.AddAsync Error: {Environment.NewLine}");
                return new RoadWorkSectionPlanResponse($"Error occured while saving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkSectionPlanResponse> FindByIdAsync(long ID)
        {
            try
            {
                var existingRoadWorkplanSection = await _roadWorkSectionPlanRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadWorkplanSection == null)
                {
                    return new RoadWorkSectionPlanResponse("No Record Found");
                }
                else
                {
                    return new RoadWorkSectionPlanResponse(existingRoadWorkplanSection);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkSectionPlanService.AddAsync Error: {Environment.NewLine}");
                return new RoadWorkSectionPlanResponse($"Error occured while retrieving the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IEnumerable<RoadWorkSectionPlan>> ListAsync(long roadId, long financialYearId)
        {
            try
            {
                return await _roadWorkSectionPlanRepository.ListAsync(roadId, financialYearId).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RoadWorkSectionPlanService.ListAsync Error: {Environment.NewLine}");
                return Enumerable.Empty<RoadWorkSectionPlan>();
            }
        }

        public async Task<IEnumerable<RoadWorkSectionPlan>> ListByAgencyAsync(long authorityId, long financialYearId)
        {
            try
            {
                return await _roadWorkSectionPlanRepository.ListByAgencyAsync(authorityId, financialYearId).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RoadWorkSectionPlanService.ListAsync Error: {Environment.NewLine}");
                return Enumerable.Empty<RoadWorkSectionPlan>();
            }
        }


        public async Task<IEnumerable<RoadWorkSectionPlan>> ListAgenciesAllAsync(long financialYearId)
        {
            try
            {
                return await _roadWorkSectionPlanRepository.ListAgenciesAllAsync(financialYearId).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"RoadWorkSectionPlanService.ListAsync Error: {Environment.NewLine}");
                return Enumerable.Empty<RoadWorkSectionPlan>();
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkSectionPlanResponse> RemoveAsync(long ID)
        {
            try
            {
                var existingRoadWorkplanSection = await _roadWorkSectionPlanRepository.FindByIdAsync(ID).ConfigureAwait(false);
                if (existingRoadWorkplanSection == null)
                {
                    return new RoadWorkSectionPlanResponse("Record NotFound");
                }
                else
                {
                    _roadWorkSectionPlanRepository.Remove(existingRoadWorkplanSection);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    return new RoadWorkSectionPlanResponse(existingRoadWorkplanSection);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkSectionPlanService.RemoveAsync Error: {Environment.NewLine}");
                return new RoadWorkSectionPlanResponse($"Error occurred while removing the record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<RoadWorkSectionPlanResponse> UpdateAsync(RoadWorkSectionPlan roadWorkSectionPlan)
        {
            try
            {
                var existingRoadWorkSection = await _roadWorkSectionPlanRepository.FindByIdAsync(roadWorkSectionPlan.ID).ConfigureAwait(false);
                if (existingRoadWorkSection == null)
                {
                    return new RoadWorkSectionPlanResponse("Record Not Found");
                }
                else
                {
                    existingRoadWorkSection.ApprovalStatus = roadWorkSectionPlan.ApprovalStatus;
                    existingRoadWorkSection.ConstituencyId = roadWorkSectionPlan.ConstituencyId;
                    existingRoadWorkSection.ExecutionMethodId = roadWorkSectionPlan.ExecutionMethodId;
                    existingRoadWorkSection.FundingSourceId = roadWorkSectionPlan.FundingSourceId;
                    existingRoadWorkSection.FundTypeId = roadWorkSectionPlan.FundTypeId;
                    existingRoadWorkSection.RevisionStatus = roadWorkSectionPlan.RevisionStatus;
                    existingRoadWorkSection.RoadSectionId = roadWorkSectionPlan.RoadSectionId;
                    existingRoadWorkSection.TotalEstimateCost = roadWorkSectionPlan.TotalEstimateCost;
                    existingRoadWorkSection.WorkCategoryId = roadWorkSectionPlan.WorkCategoryId;

                    _roadWorkSectionPlanRepository.Update(existingRoadWorkSection);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);

                    return new RoadWorkSectionPlanResponse(roadWorkSectionPlan);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkSectionPlanService.UpdateAsync Error: {Environment.NewLine}");
                return new RoadWorkSectionPlanResponse($"Error occured while updating the record : {Ex.Message}");
            }
        }

        public async Task UpdateBatchId(long financialYearId, long BatchId,long authorityId, bool isFinal)
        {
            try
            {
                //update all records for the current financial year.
                var currentYearWorkplans = await _roadWorkSectionPlanRepository.ListByAgencyAsync(authorityId, financialYearId).ConfigureAwait(false);
                //update each section plan
                foreach(var sectionPlan in currentYearWorkplans)
                {
                   
                    if (isFinal)
                    {
                        sectionPlan.ApprovalStatus = true;
                    }
                    else
                    {
                        sectionPlan.WorkplanApprovalBatchId = BatchId;
                    }
                       
                    _roadWorkSectionPlanRepository.Update(sectionPlan);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                }

            }
            catch(Exception Ex)
            {
                _logger.LogError(Ex, $"RoadWorkSectionPlanService.UpdateBatchId Error: {Environment.NewLine}");
            }
        }
    }
}
