using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<Training>> ListAsync();

        Task<IEnumerable<Training>> ListAsync(long AuthorityId);

        Task<IEnumerable<Training>> ListAsync(long AuthorityId, long FinancialYearId);

        Task<IEnumerable<Training>> ListByFinancialYearAsync(long FinancialYearId);

        Task<IEnumerable<Training>> ListByTrainingCourseInFinancialYearAsync(long TrainingCourseId, long FinancialYearId);

        Task<IEnumerable<Training>> ListDistinctCoursesByFinancialYearAsync(long FinancialYearId);
        Task<IEnumerable<TrainingCourse>> GetDistinctCoursesByFinancialYearAndAuthorityAsync(long AuthorityId, long FinancialYearId);
        Task AddAsync(Training training);
        Task<Training> FindByIdAsync(long ID);

        void Update(Training training);
        void Update(long ID, Training training);

        void Remove(Training training);

        Task<Training> FindByQuarterCodeUnitIdAndFinancialIdAsync(long QuarterCodeUnitId, long FinancialYearId);

        Task<Training> FindByQuarterCodeUnitIdAndAuthorityIdAsync(long QuarterCodeUnitId, long AuthorityId);

        Task<IEnumerable<Training>> GetCourceTrainingsThisQuarterAsync(int TrainingCourseID, int quarter);

        Task<IEnumerable<Training>> GetCourceTrainingsThisFinancialYearAsync(int TrainingCourseID, long FinancialYearId);
    }
}
