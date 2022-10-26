using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface ICOSTESRespository
    {
        Task<IActionResult> ListTokensAsync();

        Task<IActionResult> GetInstructutedWorkItemsAsync(string Token, int Year, string RegionCode);

        Task<IActionResult> EncodeTo64Async(string toEncode);

        Task<IActionResult> GetAccessTokenAsync(string Token);
    }
}
