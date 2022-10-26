using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APRP.Web.Controllers
{
    public class IssueController : Controller
    {

        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IComplaintTypeService _complaintTypeService;
        private readonly IComplaintService _complaintService;
        public IssueController(
            IApplicationUsersService applicationUsersService,
            IAuthorityService authorityService,
            IComplaintTypeService complaintTypeService,
            IComplaintService complaintService
        )
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _complaintTypeService = complaintTypeService;
            _complaintService = complaintService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new IssueViewModel();
            var user = await GetLoggedInUser().ConfigureAwait(false);
            if (user != null)
            {
                viewModel.ApplcationUser = user;
            }

            var complaintList = await _complaintService.ListAsync().ConfigureAwait(false);

            viewModel.Complaints = complaintList;
            return View(viewModel);
        }

        private async Task<ApplicationUser> GetLoggedInUser()
        {
            var userResp = await _applicationUsersService.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
            ApplicationUser user = null;
            if (userResp.Success)
            {
                //user is found


                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    user = (ApplicationUser)result2.Value;
                }
            }
            return user;
        }

        public async Task<IActionResult> ComplaintAddEditPartialView(long complaintID)
        {
            Complaint complaintModel = new Complaint();
            var complaintTypeList = await _complaintTypeService.ListAsync().ConfigureAwait(false);

            if (complaintID > 0)
            {
                var complaintResponse = await _complaintService.FindByIdAsync(complaintID).ConfigureAwait(false);
                if (complaintResponse.Success)
                    complaintModel = complaintResponse.Complaint;
            }
            ViewBag.ComplaintTypeList = new SelectList(complaintTypeList, "ID", "Code");

            return PartialView(complaintModel);

        }

        [HttpPost]
        public async Task<IActionResult> AddEditComplaint(Complaint complaint)
        {
            if (complaint != null)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
               
                if (complaint.ID > 0)
                {
                    var complaintResp = await _complaintService.FindByIdAsync(complaint.ID).ConfigureAwait(false);
                    if (complaintResp.Success)
                    {
                        var existingRecord = complaintResp.Complaint;
                        existingRecord.ResolutionComment = complaint.ResolutionComment;
                        existingRecord.DateResolved = DateTime.UtcNow;
                        existingRecord.ResolvedBy = user.UserName;
                        //editing the record
                        var updateResponse = await _complaintService.UpdateAsync(existingRecord).ConfigureAwait(false);
                        if (updateResponse.Success)
                        {
                            var id = updateResponse.Complaint.ID;
                            return Json(new
                            {
                                Success = true,
                                Message = "Success",
                                Href = Url.Action("Index", "Issue")
                            });
                        }
                        else
                        {
                            return PartialView("ComplaintAddEditPartialView", complaint);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Invalid record submitted"
                        });
                    }

                }
                else
                {
                    //is a nenw record
                    complaint.RaisedBy = user.UserName;
                    complaint.AuthorityId = user.AuthorityId;
                    var addResponse = await _complaintService.AddAsync(complaint).ConfigureAwait(false);
                    if (addResponse.Success)
                    {
                        var id = addResponse.Complaint.ID;
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("Index", "Issue")
                        });
                    }
                    else
                    {
                        return PartialView("ComplaintAddEditPartialView", complaint);
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Submitted record is null"
                });
            }
        }
    }
}