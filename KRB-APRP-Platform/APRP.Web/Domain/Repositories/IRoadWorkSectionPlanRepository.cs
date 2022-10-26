using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadWorkSectionPlanRepository
    {
        Task<IEnumerable<RoadWorkSectionPlan>> ListAsync(long roadId, long financialYearId);
        Task<IEnumerable<RoadWorkSectionPlan>> ListByAgencyAsync(long authorityId, long financialYearId);

        Task<IEnumerable<RoadWorkSectionPlan>> ListAgenciesAllAsync(long financialYearId);
        Task AddAsync(RoadWorkSectionPlan roadWorkSectionPlan);
        Task<RoadWorkSectionPlan> FindByIdAsync(long ID);
        void Update(RoadWorkSectionPlan roadWorkSectionPlan);
        void Remove(RoadWorkSectionPlan roadWorkSectionPlan);
    }
}
