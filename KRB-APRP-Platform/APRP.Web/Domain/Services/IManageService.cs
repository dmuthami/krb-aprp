using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.DTO;

namespace APRP.Web.Domain.Services
{
    public interface IManageService
    {
        Task<ManageResponse> AddPhone(UserDTO userDTO);

        Task<ManageResponse> VerifyPhone(UserDTO userDTO);

        Task<ManageResponse> VerifyPhone2(UserDTO userDTO);

        //Task<ManageResponse> SendCode2(UserDTO userDTO);

        Task<ManageResponse> SendCode3(UserDTO userDTO);

        Task<ManageResponse> AddPhone2(UserDTO userDTO);
    }
}
