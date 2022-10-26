using Microsoft.AspNetCore.Mvc;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class AllocationController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger _logger;
        private readonly IUploadService _uploadService;
        private readonly IBudgetCeilingService _budgetCeilingService;
        private readonly IAuthorityService _authorityService;
        private readonly ISurfaceTypeService _surfaceTypeService;
        private readonly IRoadClassCodeUnitService _roadClassCodeUnitService;
        private readonly IRoadConditionCodeUnitService _roadConditionCodeUnitService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IApplicationRolesService _applicationRolesService;
        private readonly ICommentService _commentService;
        private readonly ICommunicationService _communicationService;
        public readonly IRoadService _roadService;

        public AllocationController(
             ILogger<AllocationController> logger,
             IWebHostEnvironment hostingEnvironment,
              IUploadService uploadService,
              IBudgetCeilingService budgetCeilingService,
              IAuthorityService authorityService,
              ISurfaceTypeService surfaceTypeService, IRoadClassCodeUnitService roadClassCodeUnitService, IRoadConditionCodeUnitService roadConditionCodeUnitService,
              IApplicationUsersService applicationUsersService, ICommentService commentService, IApplicationRolesService applicationRolesService,
              ICommunicationService communicationService, IRoadService roadService)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _uploadService = uploadService;
            _budgetCeilingService = budgetCeilingService;
            _authorityService = authorityService;
            _surfaceTypeService = surfaceTypeService;
            _roadClassCodeUnitService = roadClassCodeUnitService;
            _roadConditionCodeUnitService = roadConditionCodeUnitService;
            _applicationUsersService = applicationUsersService;
            _commentService = commentService;
            _applicationRolesService = applicationRolesService;
            _communicationService = communicationService;
            _roadService = roadService;
        }

        #region Supporting Documents
        [Authorize(Claims.Permission.BudgetCeiling.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> SupportingDocs(long id)
        {
            try
            {
                //Create Model
                AllocationViewModel allocationViewModel = new AllocationViewModel();

                //Get BudgetCeiling
                var resp = await _budgetCeilingService.FindByIdAsync(id).ConfigureAwait(false);
                allocationViewModel.BudgetCeiling = resp.BudgetCeiling;

                //Set Referer
                allocationViewModel.Referer = Request.Headers["Referer"].ToString();

                return View(allocationViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"AllocationController.Supporting Docs Action {Ex.Message}");
                return View();
            }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.BudgetCeiling.Upload)]
        public async Task<IActionResult> UploadSupportingDocs()
        {
            try
            {
                //Create Model
                AllocationViewModel allocationViewModel = new AllocationViewModel();

                //Get BudgetCeiling ID
                long BudgetCeilingId;
                bool results = long.TryParse(Request.Form["BudgetCeilingId"].ToString(), out BudgetCeilingId);

                //Get BudgetCeiling
                var resp = await _budgetCeilingService.FindByIdAsync(BudgetCeilingId).ConfigureAwait(false);
                allocationViewModel.BudgetCeiling = resp.BudgetCeiling;

                //Upload the files
                //Check if files injected in request object
                if (Request.Form.Files.Count > 0)
                {
                    try
                    {
                        var files = Request.Form.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            var file = files[i];
                            string fname;
                            string x = Request.Headers["User-Agent"].ToString();//.Contains("Edge")
                            if (Request.Headers["User-Agent"].ToString().Contains("IE") || Request.Headers["User-Agent"].ToString().Contains("INTERNETEXPLORER")
                                || Request.Headers["User-Agent"].ToString().Contains("Edge"))
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }
                            var path = Path.Combine(
                            _hostingEnvironment.WebRootPath, "uploads", "budgets", fname);

                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream).ConfigureAwait(false);
                            }

                            //Register with file uploads
                            Upload upload = new Upload();
                            upload.filename = fname;
                            upload.ForeignId = allocationViewModel.BudgetCeiling.ID;
                            upload.type = "budgets";

                            var uploadResp = await _uploadService.AddAsync(upload).ConfigureAwait(false);
                        }

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"AllocationController.UploadSupportingDocs() File Upload Fails Error: {Ex.Message} " +
                        $"{Environment.NewLine}");
                    }
                }

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("SupportingDocs", "Allocation", new { id = BudgetCeilingId })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError($"AllocationController.UploadSupportingDocs() Action {Ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error",
                    Href = Url.Action("Index", "Allocation")
                });
            }
        }
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetAllocationUploads(string Type, long BudgetCeilingId)
        {
            try
            {
                var UploadListResponse = await _uploadService.ListAsync(Type, BudgetCeilingId).ConfigureAwait(false);
                IList<Upload> uploads = (IList<Upload>)UploadListResponse.Upload;
                return Json(uploads);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"AllocationController.GetAllocationUploads Error: {Ex.Message}");
                return Json(null);
            }
        }
        #endregion

        #region Download
        [Authorize(Claims.Permission.BudgetCeiling.Download)]
        public async Task<IActionResult> DownloadSupportingDocs(string filename, string folder)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           _hostingEnvironment.WebRootPath, "uploads",
                           folder, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory).ConfigureAwait(false);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> DeleteBudgetCeilingAttachment(long Id, string filename, long BudgetCeilingId)
        {
            try
            {
                Upload upload = null;
                //delete the file
                Boolean FileDelete = DeleteFile(filename, "budgets");

                var uploadResponse = await _uploadService.RemoveAsync(Id).ConfigureAwait(false);
                upload = uploadResponse.Upload;
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    data = upload,
                    Href = Url.Action("SupportingDocs", "Allocation", new { id = BudgetCeilingId })
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError($"AllocationController.DeleteBudgetCeilingAttachment() Error: {Ex.Message}");
                return Json(null);
            }
        }

        private Boolean DeleteFile(string filename, string subFolder)
        {
            try
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", subFolder, filename);
                FileInfo File = new FileInfo(path);

                // Check if file exists with its full path    
                if (File.Exists)
                {
                    // If file found, delete it    
                    File.Delete();
                    return false;

                }
                else return false;
            }
            catch (IOException IOExp)
            {
                string msg = $"Error Message: {IOExp.Message.ToString()} \r\n" +
                    $"Inner Exception: {IOExp.InnerException.ToString()} \r\n" +
                    $"Stack Trace:  {IOExp.StackTrace.ToString()}";
                _logger.LogError($"AllocationController.DeleteFile() Error: \r\n {msg}");
                return false;
            }

        }
        #endregion

        #region User Access

        private async Task<ApplicationUser> GetLoggedInUser()
        {
            var userResp = await _applicationUsersService.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
            ApplicationUser user = null;
            if (userResp.Success)
            {

                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    user = (ApplicationUser)result2.Value;
                    if (user != null)
                    {
                        var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        user.Authority = userAuthority.Authority;
                    }
                }
            }
            return user;
        }

        #endregion
    }
}
