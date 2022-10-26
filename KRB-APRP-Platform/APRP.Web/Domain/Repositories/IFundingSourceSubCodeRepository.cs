using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IFundingSourceSubCodeRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(FundingSourceSubCode fundingSourceSubCode);
        Task<IActionResult> FindByIdAsync(long ID);
        Task<IActionResult> Update(FundingSourceSubCode fundingSourceSubCode);
        Task<IActionResult> Update(long ID,FundingSourceSubCode fundingSourceSubCode);
        Task<IActionResult> Remove(FundingSourceSubCode fundingSourceSubCode);
    }
}
