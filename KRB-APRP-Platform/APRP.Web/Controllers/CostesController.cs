using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels.COSTES;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APRP.Web.Controllers
{
    public class CostesController : Controller
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;
        private readonly ICOSTESAPIService _cOSTESAPIService;
        private readonly IARICSApprovalService _aRICSYearService;
        private readonly IAuthorityService _authorityService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IItemActivityUnitCostService _itemActivityUnitCostService;
        private readonly IItemActivityUnitCostRateService _itemActivityUnitCostRateService;
        private readonly ICostesRegionService _costesRegionService;

        public CostesController(IConfiguration configuration, ILogger<CostesController> logger,
            ICOSTESAPIService cOSTESAPIService, IARICSApprovalService aRICSYearService,
            IItemActivityUnitCostService itemActivityUnitCostService, IAuthorityService authorityService,
             IFinancialYearService financialYearService, IItemActivityUnitCostRateService itemActivityUnitCostRateService,
             ICostesRegionService costesRegionService)
        {
            Configuration = configuration;
            _logger = logger;
            _cOSTESAPIService = cOSTESAPIService;
            _aRICSYearService = aRICSYearService;
            _itemActivityUnitCostService = itemActivityUnitCostService;
            _authorityService = authorityService;
            _financialYearService = financialYearService;
            _itemActivityUnitCostRateService = itemActivityUnitCostRateService;
            _costesRegionService = costesRegionService;
        }
        public async Task<IActionResult> GetInstructutedWorkItems(int? Year, long _FinacialYearID, string _RegionCode)
        {
            //Specify Year
            int _Year = DateTime.UtcNow.Year;
            bool resultYear = int.TryParse(Year.ToString(), out _Year);
            if (resultYear == false)
            {
                _Year = DateTime.UtcNow.Year;
            }

            //Get the region code
            if (_RegionCode == null)
            {
                _RegionCode = "1";
            }
            //Instantiate model for the View
            COSTESTViewModel cOSTESTViewModel = new COSTESTViewModel();

            //Get instructed works
            cOSTESTViewModel.GetInstructutedWorkItemsViewModel = await GetInstructutedWorkItemsList(_Year, _RegionCode).ConfigureAwait(false);

            //Convert RegionCode to Integer
            int RegionCode = 0;
            bool resultRegion = int.TryParse(_RegionCode.ToString(), out RegionCode);
            if (resultRegion == false)
            {
                RegionCode = 1;
            }

            //Convert FinancialYearID to Long
            long FinacialYearID = 0;
            bool resultFinancialYear = long.TryParse(_FinacialYearID.ToString(), out FinacialYearID);
            if (resultFinancialYear == false)
            {
                FinacialYearID = 0;//Force it to get current year
            }
            //Populate drop down
            await PopulateDropDown(_Year, RegionCode, FinacialYearID).ConfigureAwait(false);
            return View(cOSTESTViewModel);
        }

        #region CRUD

        private async Task<IList<GetInstructutedWorkItemsViewModel>> GetInstructutedWorkItemsList(int _Year, string _RegionCode)
        {

            string Token = Configuration["COSTESToken"];
            IList<GetInstructutedWorkItemsViewModel> getInstructutedWorkItemsViewModels = null;
            var costesResp = await _cOSTESAPIService.EncodeTo64Async(null).ConfigureAwait(false);
            if (costesResp.Success)
            {
                var objectResult2 = (ObjectResult)costesResp.IActionResult;
                if (objectResult2 != null)
                {
                    if (objectResult2.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult2;
                        string token = (string)result.Value;

                        //Call api to Generate Access token
                        var accessTokenResp = await _cOSTESAPIService.GetAccessTokenAsync(token).ConfigureAwait(false);
                        if (accessTokenResp.Success)
                        {
                            var objectResult3 = (ObjectResult)accessTokenResp.IActionResult;
                            if (objectResult3 != null)
                            {
                                if (objectResult3.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result3 = (OkObjectResult)objectResult3;
                                    Token = (string)result3.Value;

                                    //Hit the api to get some instructed works
                                    costesResp = await _cOSTESAPIService.GetInstructutedWorkItemsAsync(Token, _Year, _RegionCode).ConfigureAwait(false);
                                    var objectResult = (ObjectResult)costesResp.IActionResult;
                                    if (objectResult != null)
                                    {
                                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            var result4 = (OkObjectResult)objectResult;
                                            getInstructutedWorkItemsViewModels = (IList<GetInstructutedWorkItemsViewModel>)result4.Value;
                                        }
                                        else
                                        {
                                            var result4 = (OkObjectResult)objectResult;
                                            getInstructutedWorkItemsViewModels = (IList<GetInstructutedWorkItemsViewModel>)result4.Value;
                                        }
                                    }
                                }
                                else
                                {
                                    var result3 = (OkObjectResult)objectResult3;
                                    string access_token = (string)result3.Value;
                                }
                            }
                        }
                    }
                    else
                    {
                        var result = (OkObjectResult)objectResult2;
                        string access_token = (string)result.Value;
                    }
                }
            }

            return getInstructutedWorkItemsViewModels;
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> InsertInstructutedWorkItems(int? Year, string _RegionCode, long FinacialYearID )
        {
            //Specify Year
            int _Year = DateTime.UtcNow.Year;
            bool resultYear = int.TryParse(Year.ToString(), out _Year);
            if (resultYear == false)
            {
                _Year = DateTime.UtcNow.Year;
            }

            ////Set RegionCode
            int RegionCode = 0;
            bool result = int.TryParse(_RegionCode.ToString(), out RegionCode);
            if (result==false)
            {
                RegionCode = 1;
            }

            double rate = 0d;
            bool resultDouble = false;

            //Get instructed works
            IList<GetInstructutedWorkItemsViewModel> getInstructutedWorkItems = await GetInstructutedWorkItemsList(_Year, RegionCode.ToString()).ConfigureAwait(false);

            //get current financial year
            var respFinancialYearService = await _financialYearService.FindByIdAsync(FinacialYearID).ConfigureAwait(false);
            if (respFinancialYearService.Success)
            {
                //Get a list of counties
                var respAuthorities = await _authorityService.ListAsync("2").ConfigureAwait(false);
                if (respAuthorities != null)
                {
                    //loop through the list
                    foreach (var instructedWorkItem in getInstructutedWorkItems)
                    {

                        //Find ItemActivityUnitCost
                        string code = instructedWorkItem.Code;

                        var respItemActivityUnitCost = await _itemActivityUnitCostService.FindByCodeAsync(code).ConfigureAwait(false);
                        if (respItemActivityUnitCost.Success)
                        {
                            //Get itemactivityunitcostid
                            long Id = respItemActivityUnitCost.ItemActivityUnitCost.ID;

                            foreach (var authority in respAuthorities)
                            {
                                rate = 0d;
                                if (RegionCode == 1)//Nairobi/Mombasa/Kisumu
                                {
                                    if (authority.Name == "Nairobi" || authority.Name == "Kisumu" || authority.Name == "Mombasa")
                                    {
                                        //Search by itemactivityunitcostid,AuthorityID and Financial Year
                                        var resp = await _itemActivityUnitCostRateService.FindByFinancialYearAuthorityAndItemUnitCostAsync(
                                            respFinancialYearService.FinancialYear.ID, authority.ID, Id).ConfigureAwait(false);
                                        var objectResult = (ObjectResult)resp.IActionResult;
                                        if (objectResult != null)
                                        {
                                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                            {

                                                var result1 = (OkObjectResult)objectResult;
                                                var itemActivityUnitCostRate = (ItemActivityUnitCostRate)result1.Value;

                                                if (itemActivityUnitCostRate==null)
                                                {
                                                    //Add
                                                    itemActivityUnitCostRate = new ItemActivityUnitCostRate();
                                                    itemActivityUnitCostRate.ItemActivityUnitCostId = Id;
                                                    itemActivityUnitCostRate.AuthorityId = authority.ID;
                                                    itemActivityUnitCostRate.FinancialYearId = respFinancialYearService.FinancialYear.ID;
                                                    resultDouble = double.TryParse(instructedWorkItem.UnitPrice.ToString(), out rate);
                                                    itemActivityUnitCostRate.AuthorityRate = rate;
                                                    var respAdd = await _itemActivityUnitCostRateService.AddAsync(itemActivityUnitCostRate).ConfigureAwait(false);

                                                }
                                                else
                                                {
                                                    //Do nothing since the itemActivityUnitCostRate for the authority in the financial
                                                    //year exists
                                                }
                                            }
                                            else
                                            {//Add
                                                ItemActivityUnitCostRate itemActivityUnitCostRate = new ItemActivityUnitCostRate();
                                                itemActivityUnitCostRate.ItemActivityUnitCostId = Id;
                                                itemActivityUnitCostRate.AuthorityId = authority.ID;
                                                itemActivityUnitCostRate.FinancialYearId = respFinancialYearService.FinancialYear.ID;
                                                resultDouble = double.TryParse(instructedWorkItem.UnitPrice.ToString(), out rate);
                                                itemActivityUnitCostRate.AuthorityRate = rate;
                                                var respAdd = await _itemActivityUnitCostRateService.AddAsync(itemActivityUnitCostRate).ConfigureAwait(false);

                                            }
                                        }
                                    }

                                }
                                else
                                {//Other Regions

                                    if (authority.Name != "Nairobi" || authority.Name != "Kisumu" || authority.Name != "Mombasa")
                                    {
                                        //Search by itemactivityunitcostid,AuthorityID and Financial Year
                                        var resp = await _itemActivityUnitCostRateService.FindByFinancialYearAuthorityAndItemUnitCostAsync(
                                            respFinancialYearService.FinancialYear.ID, authority.ID, Id).ConfigureAwait(false);
                                        var objectResult = (ObjectResult)resp.IActionResult;
                                        if (objectResult != null)
                                        {
                                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                var result1 = (OkObjectResult)objectResult;
                                                var itemActivityUnitCostRate = (ItemActivityUnitCostRate)result1.Value;

                                                if (itemActivityUnitCostRate == null)
                                                {
                                                    //Add
                                                    itemActivityUnitCostRate = new ItemActivityUnitCostRate();
                                                    itemActivityUnitCostRate.ItemActivityUnitCostId = Id;
                                                    itemActivityUnitCostRate.AuthorityId = authority.ID;
                                                    itemActivityUnitCostRate.FinancialYearId = respFinancialYearService.FinancialYear.ID;
                                                    resultDouble = double.TryParse(instructedWorkItem.UnitPrice.ToString(), out rate);
                                                    itemActivityUnitCostRate.AuthorityRate = rate;
                                                    var respAdd = await _itemActivityUnitCostRateService.AddAsync(itemActivityUnitCostRate).ConfigureAwait(false);

                                                }
                                                else
                                                {
                                                    //Do nothing since the itemActivityUnitCostRate for the authority in the financial
                                                    //year exists
                                                }

                                            }
                                            else
                                            {//Add
                                                ItemActivityUnitCostRate itemActivityUnitCostRate = new ItemActivityUnitCostRate();
                                                itemActivityUnitCostRate.ItemActivityUnitCostId = Id;
                                                itemActivityUnitCostRate.AuthorityId = authority.ID;
                                                itemActivityUnitCostRate.FinancialYearId = respFinancialYearService.FinancialYear.ID;
                                                resultDouble = double.TryParse(instructedWorkItem.UnitPrice.ToString(), out rate);
                                                itemActivityUnitCostRate.AuthorityRate = rate;
                                                var respAdd = await _itemActivityUnitCostRateService.AddAsync(itemActivityUnitCostRate).ConfigureAwait(false);

                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }

            return Json(new
            {
                Success = true,
                Message = "Success"
            });
        }

        #endregion

        #region Utilities
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetInstructutedWorkItemsByYearByRegionCode(int? Year, long _FinacialYearID, string _RegionCode)
        {
            try
            {
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("GetInstructutedWorkItems", "Costes", new { Year = Year, _FinacialYearID= _FinacialYearID, _RegionCode = _RegionCode })
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkplanController.GetPrioritizationYear Error {Environment.NewLine}");
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("GetInstructutedWorkItems", "Costes", new { Year = Year, _FinacialYearID= _FinacialYearID, _RegionCode = _RegionCode })
                });
            }
        }
        private async Task PopulateDropDown(int _Year, int _RegionCode, long FinacialYearID)
        {

            //ARICS Year
            IList<ARICSYear> aRICSYears = null;
            var aricsYearResp = await _aRICSYearService.ListAsync().ConfigureAwait(false);
            if (aricsYearResp.Success)
            {
                var objectResult = (ObjectResult)aricsYearResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        aRICSYears = (IList<ARICSYear>)result.Value;
                    }
                }
            }
            ViewData["ARICSYearId"] = new SelectList(aRICSYears, "Year", "Year", _Year);
            
            //Costes Regions
            IList<CostesRegion> costesRegions = null;

            var costesRegionResp = await _costesRegionService.ListAsync().ConfigureAwait(false);
            if (costesRegionResp.Success)
            {
                var objectResult2 = (ObjectResult)costesRegionResp.IActionResult;
                if (objectResult2 != null)
                {
                    if (objectResult2.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult2;
                        costesRegions = (IList<CostesRegion>)result2.Value;
                    }
                }
            }
            ViewData["CostesRegionId"] = new SelectList(costesRegions, "Code", "Description", _RegionCode);

            //Financial Year
            //Set Financial Year
            var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
            IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
            var newFinancialYearsList = financialYears
            .OrderByDescending(v => v.Code)
            .Select(
            p => new
            {
                ID = p.ID,
                Code = $"{p.Code}.{p.Revision}"
            }
                ).ToList();

            //get current financial year
            if (FinacialYearID==0)
            {
                var respFinancialYearService = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (respFinancialYearService.Success)
                {
                    ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", respFinancialYearService.FinancialYear.ID);
                }else
                {
                    ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code");
                }
            } else
            {
                var respFinancialYearService = await _financialYearService.FindByIdAsync(FinacialYearID).ConfigureAwait(false);
                if (respFinancialYearService.Success)
                {
                    ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", respFinancialYearService.FinancialYear.ID);
                }
                else
                {
                    ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code");
                }
            }          
        }
        #endregion
    }
}
