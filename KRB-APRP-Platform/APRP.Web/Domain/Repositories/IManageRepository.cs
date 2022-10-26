using APRP.Web.ViewModels.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IManageRepository
    {
        Task<IActionResult> AddPhone(UserDTO userDTO);

        Task<IActionResult> VerifyPhone(UserDTO userDTO);

        //Task<IActionResult> SendCode2(UserDTO userDTO);

    }
}
