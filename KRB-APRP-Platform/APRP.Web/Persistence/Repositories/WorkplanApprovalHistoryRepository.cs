using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;

namespace APRP.Web.Persistence.Repositories
{
    public class WorkplanApprovalHistoryRepository : BaseRepository, IWorkplanApprovalHistoryRepository
    {
        public WorkplanApprovalHistoryRepository (AppDbContext appDbContext ) : base (appDbContext) 
        { 
        
        }

        public async Task AddAsync(WorkplanApprovalHistory workplanApprovalHistory)
        {
            await _context.WorkplanApprovalHistories.AddAsync(workplanApprovalHistory);
        }
    }
}
