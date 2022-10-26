using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ICOSTESAPIService
    {
        Task<GenericResponse> GetInstructutedWorkItemsAsync(string Token,int Year,string RegionCode);

        Task<GenericResponse> EncodeTo64Async(string toEncode);

        Task<GenericResponse> GetAccessTokenAsync(string Token);
    }
}
