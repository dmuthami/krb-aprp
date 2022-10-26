using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class TrainingController : Controller
    {
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IQuarterCodeUnitService _quarterCodeUnitService;
        private readonly ITrainingService _trainingService;
        private readonly IFinancialYearService _financialYearService;
        private readonly ITrainingCourseService _trainingCourseService;

        private readonly IMemoryCache _cache;


        public TrainingController(ILogger<TrainingController> logger,
             IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
             IQuarterCodeUnitService quarterCodeUnitService,
             IFinancialYearService financialYearService,
             ITrainingService trainingService,
             ITrainingCourseService trainingCourseService,
             IMemoryCache cache)
        {
            _logger = logger;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _quarterCodeUnitService = quarterCodeUnitService;
            _trainingService = trainingService;
            _trainingCourseService = trainingCourseService;
            _financialYearService = financialYearService;
            _cache = cache;

        }

        #region Training
        [HttpPost]
        public async Task<IActionResult> GetQuarterCodeUnitsForFinancialYear(long FinancialYearID)
        {
            //Call service to removethe revenue item from the financial year
            var quarterCodeUnitResponse = await _quarterCodeUnitService.ListAsync(FinancialYearID).ConfigureAwait(false);

            IList<QuarterCodeUnitViewModel> quarterCodeUnitList;
            quarterCodeUnitList = quarterCodeUnitResponse.QuarterCodeUnit.Select(u => new QuarterCodeUnitViewModel
            {
                ID = u.ID,
                Name = u.QuarterCodeList.Name
            }
            ).ToList();


            return Json(new
            {
                Success = quarterCodeUnitResponse.Success,
                Message = "Success",
                data = quarterCodeUnitList,
                Href = Url.Action("Index", "RevenueCollection")
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Training.View)]
        public async Task<IActionResult> Index(long? FinancialYearId)
        {
            try
            {
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                //Get current financial year
                var respFinancialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (FinancialYearId != null)
                {
                    long _FinancialYearId;
                    bool result = long.TryParse(FinancialYearId.ToString(), out _FinancialYearId);

                    respFinancialYear = await _financialYearService.FindByIdAsync(_FinancialYearId).ConfigureAwait(false);
                }

                //financial year drop down
                await IndexDropDowns(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);


                //Set Authority
                TrainingViewModel trainingViewModel = new TrainingViewModel();
                trainingViewModel.Authority = _ApplicationUser.Authority;

                //if (trainingViewModel.Authority.Code == "krb")
                //{
                //    var trainingResp = await _trainingService
                //    .ListByFinancialYearAsync(respFinancialYear.FinancialYear.ID)
                //    .ConfigureAwait(false);
                //    trainingViewModel.Trainings = (IList<Training>)trainingResp.Training;
                //}
                //else
                //{
                //    var trainingResp = await _trainingService
                //        .ListAsync(trainingViewModel.Authority.ID,respFinancialYear.FinancialYear.ID)
                //        .ConfigureAwait(false);
                //    trainingViewModel.Trainings = (IList<Training>)trainingResp.Training;
                //}


                return View(trainingViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");

                return View();
            }
        }

        [Authorize(Claims.Permission.Training.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> TrainingActivities(long? FinancialYearId)
        {
            try
            {
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                //Get current financial year
                var respFinancialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

                if (FinancialYearId != null)
                {
                    long _FinancialYearId;
                    bool result = long.TryParse(FinancialYearId.ToString(), out _FinancialYearId);

                    respFinancialYear = await _financialYearService.FindByIdAsync(_FinancialYearId).ConfigureAwait(false);
                }

                //Set Authority
                TrainingViewModel trainingViewModel = new TrainingViewModel();
                trainingViewModel.Authority = _ApplicationUser.Authority;

                //if (trainingViewModel.Authority.Code == "krb")
                //{
                //    var trainingResp = await _trainingService
                //    .ListByFinancialYearAsync(respFinancialYear.FinancialYear.ID)
                //    .ConfigureAwait(false);
                //    trainingViewModel.Trainings = (IList<Training>)trainingResp.Training;
                //}
                //else
                //{
                //    var trainingResp = await _trainingService
                //        .ListAsync(trainingViewModel.Authority.ID, respFinancialYear.FinancialYear.ID)
                //        .ConfigureAwait(false);
                //    trainingViewModel.Trainings = (IList<Training>)trainingResp.Training;
                //}

                var trainingResp = await _trainingService
                .ListAsync(trainingViewModel.Authority.ID, respFinancialYear.FinancialYear.ID)
                .ConfigureAwait(false);

                trainingViewModel.Trainings = (IList<Training>)trainingResp.Training;
                return PartialView("TrainingPartialView", trainingViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "TrainingController Index Page has reloaded");
                return View();
            }
        }
        #endregion

        #region Reporting

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Training.Report)]
        public async Task<JsonResult> ExportCumulativeTrainingReportPerFinancialYr(long FinancialYearID)
        {
            try
            {
                //Instatntiate training viewmodel
                TrainingViewModel trainingViewModel = new TrainingViewModel();

                //Get financial year
                var respFinancialYear = await _financialYearService.FindByIdAsync(FinancialYearID).ConfigureAwait(false);
                if (respFinancialYear.Success)
                {
                    trainingViewModel.FinancialYear = respFinancialYear.FinancialYear;
                }

                //Get user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                trainingViewModel.Authority = _ApplicationUser.Authority;

                //Get training for the year for the specific agency
                //var trainingResp = await _trainingService.ListByFinancialYearAsync(FinancialYearID).ConfigureAwait(false);

                var trainingResp = await _trainingService
                .ListAsync(_ApplicationUser.Authority.ID, FinancialYearID)
                .ConfigureAwait(false);
                trainingViewModel.Trainings = trainingResp.Training;

                //Get a unique list of course titles
                var trainingCourseresp = await _trainingService
                    .GetDistinctCoursesByFinancialYearAndAuthorityAsync(trainingViewModel.Authority.ID, FinancialYearID)
                    .ConfigureAwait(false);
                trainingViewModel.TrainingCourses = trainingCourseresp.TrainingCourse;

                //Write to Excel
                MemoryStream stream = new MemoryStream();
                JsonResult myjson = await WriteToExcel(trainingViewModel, stream).ConfigureAwait(false);

                return Json(myjson);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.ExportARICSSummaryReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToExcel(TrainingViewModel trainingViewModel, MemoryStream stream)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "Training_Report";
                excelPackage.Workbook.Properties.Subject = "Training Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Training");

                //Add some text to cell B4
                worksheet.Cells["B3"].Value = "TRAINING REPORT FORMAT ";
                worksheet.Cells["B3"].Style.Font.Bold = true;
                worksheet.Cells["B3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Agency
                worksheet.Cells["B4"].Value = "Agency";
                worksheet.Cells["B4"].Style.Font.Bold = true;
                worksheet.Cells["B4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C4"].Value = $"{trainingViewModel.Authority.Code}-{trainingViewModel.Authority.Name}"; 
                worksheet.Cells["C4"].Style.Font.Bold = true;
                worksheet.Cells["C4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["D4"].Value = "Financial Year ";
                worksheet.Cells["D4"].Style.Font.Bold = true;
                worksheet.Cells["D4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Get Financial year
                worksheet.Cells["E4"].Value = trainingViewModel.FinancialYear.Code;
                worksheet.Cells["E4"].Style.Font.Bold = true;
                worksheet.Cells["E4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A6"].Value = "NO.";
                worksheet.Cells["A6"].Style.Font.Bold = true;
                worksheet.Cells["A6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["B6"].Value = "TRAINING PROGRAMME /COURSE TITLE";
                worksheet.Cells["B6"].Style.Font.Bold = true;
                worksheet.Cells["B6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["C6"].Value = "CADRE OF TRAINEES";
                worksheet.Cells["C6"].Style.Font.Bold = true;
                worksheet.Cells["C6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["D6"].Value = "NO. TRAINED THIS QUARTER";
                worksheet.Cells["D6"].Style.Font.Bold = true;
                worksheet.Cells["D6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["E6"].Value = "TRAINING EXPENDITURE THIS QUARTER";
                worksheet.Cells["E6"].Style.Font.Bold = true;
                worksheet.Cells["E6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["F6"].Value = "CUMULATIVE NO. TRAINED THIS FY";
                worksheet.Cells["F6"].Style.Font.Bold = true;
                worksheet.Cells["F6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["G6"].Value = "CUMULATIVE EXPENDITURE THIS FY";
                worksheet.Cells["G6"].Style.Font.Bold = true;
                worksheet.Cells["G6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //using (ExcelRange Rng = worksheet.Cells["A6:G6"])
                //{
                //    Rng.Style.Font.Bold = true;
                //    Rng.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //}
                //You could also use [line, column] notation:
                //worksheet.Cells[1, 2].Value = "This is cell B1!";

                int i = 7;
                double sumKm = 0.0;
                //Loop through Roadsheets

                //Get Quarter
                int thisQuarter = GetQuarter();

                int k = 1;//Counter

                //Summations
                int totalNoTrainedThisQuarter = 0;
                double SumTrainingExpenditureThisQuarter = 0D;
                int totalcumulativeTrainedThisFY = 0;
                double SumCumulativeExpenditureThisFY = 0D;

                foreach (var training in trainingViewModel.TrainingCourses)
                {

                    //Sheet No
                    worksheet.Cells[i, 1].Value = k;
                    worksheet.Cells[i, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //Get Course Title
                    try
                    {
                        worksheet.Cells[i, 2].Value = training.Course;
                        worksheet.Cells[i, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[i, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"TrainingController.WriteToExcel Error {Environment.NewLine}");
                    }

                    //Get Cadre of Trainings
                    try
                    {
                        //Call function that returns unique cadre of trainings comma separated
                        worksheet.Cells[i, 3].Value = await GetCadreOfTrainees(training, trainingViewModel.FinancialYear.ID).ConfigureAwait(false);
                        worksheet.Cells[i, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[i, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"TrainingController.WriteToExcel Error {Environment.NewLine}");
                    }

                    //Get cumulative of the no trained this quarter for the specified course
                    var trainingResp = await _trainingService.GetCourceTrainingsThisQuarterAsync(training.ID, thisQuarter).ConfigureAwait(false);
                    IEnumerable<Training> courseResult = trainingResp.Training;
                    try
                    {
                        int sum = courseResult.Sum(s => s.NoTrained);
                        worksheet.Cells[i, 4].Value = sum;
                        worksheet.Cells[i, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[i, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        totalNoTrainedThisQuarter += sum;
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"TrainingController.WriteToExcel Error {Environment.NewLine}");
                    }

                    //Get Training Expenditure this quarter for this course
                    try
                    {
                        var respTrainingExpenditureThisQuarterForThisCourse = await _trainingService.GetCourceTrainingsThisQuarterAsync(
                            training.ID, thisQuarter).ConfigureAwait(false);

                        double exp = respTrainingExpenditureThisQuarterForThisCourse.Training.Sum(s => s.TrainingExpenditure);
                        worksheet.Cells[i, 5].Value = string.Format("{0:0,0.00}", exp);
                        worksheet.Cells[i, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[i, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        SumTrainingExpenditureThisQuarter += exp;
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"TrainingController.WriteToExcel Error {Environment.NewLine}");
                    }

                    //Get cummulative no. tained this financial year
                    trainingResp = await _trainingService.GetCourceTrainingsThisFinancialYearAsync(training.ID, trainingViewModel.FinancialYear.ID).ConfigureAwait(false);
                    courseResult = trainingResp.Training;
                    try
                    {
                        int trained = courseResult.Sum(s => s.NoTrained);
                        worksheet.Cells[i, 6].Value = trained;
                        worksheet.Cells[i, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[i, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        totalcumulativeTrainedThisFY += trained;
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"TrainingController.WriteToExcel Error {Environment.NewLine}");
                    }


                    //Get cummulativeexpenditure this financial year
                    trainingResp = await _trainingService.GetCourceTrainingsThisFinancialYearAsync(training.ID, trainingViewModel.FinancialYear.ID).ConfigureAwait(false);
                    courseResult = trainingResp.Training;
                    try
                    {
                        double exp2 = courseResult.Sum(s => s.TrainingExpenditure);
                        worksheet.Cells[i, 7].Value = string.Format("{0:0,0.00}", exp2);
                        worksheet.Cells[i, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksheet.Cells[i, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        SumCumulativeExpenditureThisFY += exp2;

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"TrainingController.WriteToExcel Error {Environment.NewLine}");
                    }
                    //iterate
                    i++;
                    k++;
                }

                worksheet.Cells["B" + i].Value = "Total";
                worksheet.Cells["B" + i].Style.Font.Bold = true;
                worksheet.Cells["B" + i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                worksheet.Cells["B" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Sum : NO. TRAINED THIS QUARTER
                worksheet.Cells["D" + i].Value = totalNoTrainedThisQuarter;
                worksheet.Cells["D" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Sum : TRAINING EXPENDITURE THIS QUARTER
                worksheet.Cells["E" + i].Value = String.Format(new CultureInfo("sw-KE"), "{0:C}", SumTrainingExpenditureThisQuarter);
                worksheet.Cells["E" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Sum : CUMULATIVE NO. TRAINED THIS FY
                worksheet.Cells["F" + i].Value = totalcumulativeTrainedThisFY;
                worksheet.Cells["F" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Sum : CUMULATIVE EXPENDITURE THIS FY
                worksheet.Cells["G" + i].Value = string.Format("{0:0,0.00}", SumCumulativeExpenditureThisFY);
                //worksheet.Cells["G" + i].Value = String.Format(new CultureInfo("sw-KE"), "{0:C}", SumCumulativeExpenditureThisFY);
                worksheet.Cells["G" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A" + i + ":G" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + i + ":G" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + i + ":G" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A" + i + ":G" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Autofit columns
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                worksheet.Column(3).Width = 30;
                worksheet.Column(3).Style.WrapText = true;

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download", "Training", new { fileGuid = handle, FileName = "Training_Report.xlsx" })
                });

            }
            //stream.Close();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<string> GetCadreOfTrainees(TrainingCourse trainingCourse, long FinancialYearId)
        {
            //Get all trainings for the training cource in the specified financial year
            var trainingResp = await _trainingService.ListByTrainingCourseInFinancialYearAsync(trainingCourse.ID, FinancialYearId).ConfigureAwait(false);
            Dictionary<string, string> cadreOfTraineesDict = new Dictionary<string, string>();
            if (trainingResp.Success)
            {
                foreach (var training in trainingResp.Training)
                {
                    try
                    {
                        cadreOfTraineesDict.Add(training.CadreOfTrainees, training.CadreOfTrainees);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"TrainingController.GetCadreOfTrainees:{Environment.NewLine}");
                    }
                }
            }
            return string.Join(",", cadreOfTraineesDict.Keys.ToList());
            ;
        }

        public ActionResult Download(string fileGuid, string fileName)
        {

            if (_cache.Get<byte[]>(fileGuid) != null)
            {
                byte[] data = _cache.Get<byte[]>(fileGuid);
                _cache.Remove(fileGuid); //cleanup here as we don't need it in cache anymore
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Something has gone wrong...
                return View("Error"); // or whatever/wherever you want to return the user
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private int GetQuarter()
        {
            try
            {
                //Get current date
                DateTime today = DateTime.UtcNow;

                //1st quarter-July/Aug/Sept
                if (today.Month == 7 || today.Month == 8 || today.Month == 9)
                {
                    return 1;
                }
                //2nd Quarter-Oct-Nov-Dec
                if (today.Month == 10 || today.Month == 11 || today.Month == 12)
                {
                    return 2;
                }
                //3rd Quarter-Jan-Feb-Mar
                if (today.Month == 1 || today.Month == 2 || today.Month == 3)
                {
                    return 3;
                }
                //4th Quarter-Apr-May-June
                return 4;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingController.GetQuarter Error {Environment.NewLine}");
                return 0;
            }
        }
        #endregion

        #region Utilities

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
                    if (user != null)
                    {
                        var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        user.Authority = userAuthority.Authority;
                    }
                }
            }
            return user;
        }

        private async Task IndexDropDowns(long FinancialYearId)
        {
            //Set Financial Year
            var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
            IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
            var newFinancialYearsList = financialYears
                .OrderByDescending(v => v.Code)
                .Select(
                p => new
                {
                    ID = p.ID,
                    Code = $"{p.Code}-{p.Revision}"
                }
                    ).ToList();
            if (FinancialYearId == 0)
            {
                ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code");
            }
            else
            {
                ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", FinancialYearId);
            }

        }
        #endregion

    }
}