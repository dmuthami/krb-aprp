using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IDisbursementCodeListRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(DisbursementCodeList disbursementCodeList);
        Task<IActionResult> FindByIdAsync(long ID);
        Task<IActionResult> FindByCodeAsync(string Code);
        Task<IActionResult> Update(DisbursementCodeList disbursementCodeList);
        Task<IActionResult> Update(long ID,DisbursementCodeList disbursementCodeList);
        Task<IActionResult> Remove(DisbursementCodeList disbursementCodeList);
        Task<IActionResult> DetachFirstEntryAsync(DisbursementCodeList disbursementCodeList);
        Task<IActionResult> FindByDisbursementCodeListEntryAsync(DisbursementCodeList disbursementCodeList);
    }
}
