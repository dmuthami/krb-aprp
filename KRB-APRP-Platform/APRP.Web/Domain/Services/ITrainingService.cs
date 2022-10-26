using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ITrainingService
    {
        Task<TrainingListResponse> ListAsync();
        Task<TrainingListResponse> ListAsync(long AuthorityId);
        Task<TrainingListResponse> ListAsync(long AuthorityId, long FinancialYearId);
        Task<TrainingListResponse> ListByFinancialYearAsync(long FinancialYearId);
        Task<TrainingListResponse> ListByTrainingCourseInFinancialYearAsync(long TrainingCourseId,long FinancialYearId);
        Task<TrainingListResponse> ListDistinctCoursesByFinancialYearAsync(long FinancialYearId);
        Task<TrainingCourseListResponse> GetDistinctCoursesByFinancialYearAndAuthorityAsync(long AuthorityId,long FinancialYearId);
        Task<TrainingResponse> AddAsync(Training training);
        Task<TrainingResponse> FindByIdAsync(long ID);
        Task<TrainingResponse> Update(Training training);
        Task<TrainingResponse> Update(long ID, Training training);
        Task<TrainingResponse> RemoveAsync(long ID);
        Task<TrainingResponse> FindByQuarterCodeUnitIdAndFinancialIdAsync(long QuarterCodeUnitId, long FinancialYearId);
        Task<TrainingResponse> FindByQuarterCodeUnitIdAndAuthorityIdAsync(long QuarterCodeUnitId, long AuthorityId);
        Task<TrainingListResponse> GetCourceTrainingsThisQuarterAsync(int TrainingCourseID, int quarter);
        Task<TrainingListResponse> GetCourceTrainingsThisFinancialYearAsync(int TrainingCourseID, long FinancialYearId);
    }
}
