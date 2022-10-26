using Microsoft.AspNetCore.Mvc;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.Domain.Services;
using Microsoft.Extensions.Caching.Memory;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class RevenueCollectionCodeUnitTypesController : Controller
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRevenueCollectionCodeUnitTypeService _revenueCollectionCodeUnitTypeService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;
        private readonly IUploadService _uploadService;
        private readonly IRevenueCollectionCodeUnitService _revenueCollectionCodeUnitService;

        public RevenueCollectionCodeUnitTypesController(
            IApplicationUsersService applicationUsersService,
            IAuthorityService authorityService,
            ILogger<RevenueCollectionCodeUnitTypesController> logger,
            IRevenueCollectionCodeUnitTypeService revenueCollectionCodeUnitTypeService,
            IUploadService uploadService, IWebHostEnvironment hostingEnvironment,
            IMemoryCache cache, IRevenueCollectionCodeUnitService revenueCollectionCodeUnitService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _revenueCollectionCodeUnitTypeService = revenueCollectionCodeUnitTypeService;
            _logger = logger;
            _cache = cache;
            _hostingEnvironment = hostingEnvironment;
            _uploadService = uploadService;
            _revenueCollectionCodeUnitService = revenueCollectionCodeUnitService;
        }


        // GET: RevenueCollectionCodeUnitTypes
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnitType.View)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var resp = await _revenueCollectionCodeUnitTypeService.ListAsync().ConfigureAwait(false);
                return View(resp.RevenueCollectionCodeUnitType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitType Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionCodeUnitType Index Page has reloaded");
                return View();
            }
        }


        // GET: RevenueCollectionCodeUnitTypes/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnitType.View)]
        public async Task<IActionResult> Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _revenueCollectionCodeUnitTypeService.FindByIdAsync(ID).ConfigureAwait(false);
                var revenueCollectionCodeUnitType = resp.RevenueCollectionCodeUnitType;
                if (revenueCollectionCodeUnitType == null)
                {
                    return NotFound();
                }

                return View(revenueCollectionCodeUnitType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitType Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionCodeUnitType Details Page has reloaded");
                return View();
            }
        }

        // GET: RevenueCollectionCodeUnitTypes/Create
        [Authorize(Claims.Permission.RevenueCollectionCodeUnitType.View)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: RevenueCollectionCodeUnitTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnitType.Add)]
        public async Task<IActionResult> Create([Bind("ID,Type")] RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            try
            {
                var resp = await _revenueCollectionCodeUnitTypeService.FindByNameAsync(revenueCollectionCodeUnitType.Type).ConfigureAwait(false);
                if (resp.RevenueCollectionCodeUnitType != null)
                {
                    var revenueCollectionCodeUnitType2 = resp.RevenueCollectionCodeUnitType;
                    if (revenueCollectionCodeUnitType2.Type == "KRB" || revenueCollectionCodeUnitType2.Type == "Others")
                    {
                        string msg = $"Type may not be named {revenueCollectionCodeUnitType2.Type} " +
                            $"Contact system Administrator if the aforementioned type is missing from the system";
                        ModelState.AddModelError(string.Empty, msg);
                        return View(revenueCollectionCodeUnitType);
                    }
                    //Detach the first entry before attaching your updated entry
                    var respDetach = await _revenueCollectionCodeUnitTypeService.DetachFirstEntryAsync(resp.RevenueCollectionCodeUnitType).ConfigureAwait(false);
                }

                if (ModelState.IsValid)
                {
                    var resp2 = await _revenueCollectionCodeUnitTypeService.AddAsync(revenueCollectionCodeUnitType).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                return View(revenueCollectionCodeUnitType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitType Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionCodeUnitType Create Page has reloaded");
                return View(revenueCollectionCodeUnitType);
            }
        }


        // GET: RevenueCollectionCodeUnitTypes/Edit/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnitType.View)]
        public async Task<IActionResult> Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _revenueCollectionCodeUnitTypeService.FindByIdAsync(ID).ConfigureAwait(false);
                var revenueCollectionCodeUnitType = resp.RevenueCollectionCodeUnitType;
                if (revenueCollectionCodeUnitType == null)
                {
                    return NotFound();
                }

                return View(revenueCollectionCodeUnitType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitType Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionCodeUnitType Edit Page has reloaded");
                return View();
            }
        }

        // POST: RevenueCollectionCodeUnitTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnitType.Change)]
        public async Task<IActionResult> Edit(long id, [Bind("ID,Type")] RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            try
            {
                if (id != revenueCollectionCodeUnitType.ID)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _revenueCollectionCodeUnitTypeService.FindByNameAsync(revenueCollectionCodeUnitType.Type).ConfigureAwait(false);
                if (resp.RevenueCollectionCodeUnitType != null)
                {
                    var revenueCollectionCodeUnitType2 = resp.RevenueCollectionCodeUnitType;
                    if (revenueCollectionCodeUnitType2.Type == "KRB" || revenueCollectionCodeUnitType2.Type == "Others")
                    {
                        string msg = $"Type may not be named {revenueCollectionCodeUnitType2.Type}" +
                            $"Contact system Administrator if the aforementioned type is missing from the system";
                        ModelState.AddModelError(string.Empty, msg);
                        return View(revenueCollectionCodeUnitType);
                    }
                    //Detach the first entry before attaching your updated entry
                    var respDetach = await _revenueCollectionCodeUnitTypeService.DetachFirstEntryAsync(resp.RevenueCollectionCodeUnitType).ConfigureAwait(false);
                }

                resp = await _revenueCollectionCodeUnitTypeService.FindByIdAsync(ID).ConfigureAwait(false);
                if (resp.RevenueCollectionCodeUnitType != null)
                {
                    var revenueCollectionCodeUnitType2 = resp.RevenueCollectionCodeUnitType;
                    if (revenueCollectionCodeUnitType2.Type == "KRB" || revenueCollectionCodeUnitType2.Type == "Others")
                    {
                        string msg = $"Type {revenueCollectionCodeUnitType2.Type} may not be edited" +
                            $"Contact system Administrator if the aforementioned type is missing from the system";
                        ModelState.AddModelError(string.Empty, msg);
                        return View(revenueCollectionCodeUnitType);
                    }

                    //Detach the first entry before attaching your updated entry
                    var respDetach = await _revenueCollectionCodeUnitTypeService.DetachFirstEntryAsync(resp.RevenueCollectionCodeUnitType).ConfigureAwait(false);
                }

                if (ModelState.IsValid)
                {
                    resp = await _revenueCollectionCodeUnitTypeService.Update(id, revenueCollectionCodeUnitType).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                return View(revenueCollectionCodeUnitType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Class Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Revenue Collection Code Unit Page has reloaded");
                return View(revenueCollectionCodeUnitType);
            }
        }

        // GET: RevenueCollectionCodeUnitTypes/Delete/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnitType.View)]
        public async Task<IActionResult> Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _revenueCollectionCodeUnitTypeService.FindByIdAsync(ID).ConfigureAwait(false);
                var revenueCollectionCodeUnitType = resp.RevenueCollectionCodeUnitType;
                if (revenueCollectionCodeUnitType == null)
                {
                    return NotFound();
                }

                return View(revenueCollectionCodeUnitType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitType Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionCodeUnitType Delete Page has reloaded");
                return View();
            }
        }

        // POST: RevenueCollectionCodeUnitTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnitType.Delete)]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var resp = await _revenueCollectionCodeUnitTypeService.FindByIdAsync(id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return NotFound();
                }
                resp = await _revenueCollectionCodeUnitTypeService.FindByIdAsync(id).ConfigureAwait(false);
                if (resp.RevenueCollectionCodeUnitType != null)
                {
                    var revenueCollectionCodeUnitType2 = resp.RevenueCollectionCodeUnitType;
                    if (revenueCollectionCodeUnitType2.Type == "KRB" || revenueCollectionCodeUnitType2.Type == "Others")
                    {
                        string msg = $"Type {revenueCollectionCodeUnitType2.Type} may not be deleted" +
                            $"Contact system Administrator if the aforementioned type needs to be deleted";
                        ModelState.AddModelError(string.Empty, msg);
                        return View(revenueCollectionCodeUnitType2);
                    }

                    //Detach the first entry before attaching your updated entry
                    var respDetach = await _revenueCollectionCodeUnitTypeService.DetachFirstEntryAsync(resp.RevenueCollectionCodeUnitType).ConfigureAwait(false);
                }

                resp = await _revenueCollectionCodeUnitTypeService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitType Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionCodeUnitType Delete Page has reloaded");
                return View();
            }
        }

        #region Upload & Get Documents
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UploadSupportingDocs()
        {
            try
            {
                //Get RoadId
                long RevenueCollectionCodeUnitId;
                bool results = long.TryParse(Request.Form["RevenueCollectionCodeUnitId"].ToString(), out RevenueCollectionCodeUnitId);

                //Get RevenueCollection code unit
                var roadResp = await _revenueCollectionCodeUnitService.FindByIdAsync(RevenueCollectionCodeUnitId).ConfigureAwait(false);
                RevenueCollectionCodeUnit revenueCollectionCodeUnit = roadResp.RevenueCollectionCodeUnit;

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
                            _hostingEnvironment.WebRootPath, "uploads", "FundingSource", fname);

                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream).ConfigureAwait(false);
                            }

                            //Register with file uploads
                            Upload upload = new Upload();
                            upload.filename = fname;
                            upload.ForeignId = revenueCollectionCodeUnit.ID;
                            upload.type = "FundingSource";

                            var uploadResp = await _uploadService.AddAsync(upload).ConfigureAwait(false);
                        }

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"SubmitBudget() File Upload Fails Error: {Ex.Message} " +
                        $"{Environment.NewLine}");
                    }
                }
                string referer = Request.Headers["Referer"].ToString();

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Referer = referer,
                    Href = "RevenueCollectioncodeUnitRx/Details?id=" + revenueCollectionCodeUnit.ID
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError($"RevenueCollectionCodeUnitTypes.UploadSupportingDocs Action {Ex.Message}");
                return Json(false);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetPrioritizationUploads(string Type, long RevenueCollectionCodeUnitId)
        {
            try
            {
                var UploadListResponse = await _uploadService.ListAsync(Type, RevenueCollectionCodeUnitId).ConfigureAwait(false);
                IList<Upload> ARICSUpload = (IList<Upload>)UploadListResponse.Upload;
                return Json(ARICSUpload);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"RevenueCollectionCodeUnitTypes.GetPrioritizationUploads Error: {Ex.Message}");
                return Json(null);
            }
        }
        #endregion

        #region Download & Delete Documents

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
        public async Task<JsonResult> DeleteFundingSourceAttachment(long Id, string filename, long RevenueCollectionCodeUnitId)
        {
            try
            {
                Upload upload = null;
                //delete the file
                Boolean FileDelete = DeleteFile(filename, "FundingSource");

                var uploadResponse = await _uploadService.RemoveAsync(Id).ConfigureAwait(false);
                upload = uploadResponse.Upload;

                string referer = Request.Headers["Referer"].ToString();

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    data = upload,
                    Referer = referer,
                    Href = "RevenueCollectioncodeUnitRx/Details?id=" + RevenueCollectionCodeUnitId
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError($"RevenueCollectionCodeUnitTypes.DeleteFundingSourceAttachment Error: {Ex.Message}");
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
                _logger.LogError($"RevenueCollectionCodeUnitTypes.DeleteFile: \r\n {msg}");
                return false;
            }

        }

        #endregion
    }
}
