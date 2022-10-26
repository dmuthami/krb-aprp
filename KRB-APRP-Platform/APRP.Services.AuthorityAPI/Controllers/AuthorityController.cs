using APRP.Services.AuthorityAPI.Filter;
using APRP.Services.AuthorityAPI.Helpers;
using APRP.Services.AuthorityAPI.Models;
using APRP.Services.AuthorityAPI.Models.Dto;
using APRP.Services.AuthorityAPI.Services;
using APRP.Services.AuthorityAPI.ViewModels;
using APRP.Services.AuthorityAPI.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.Metrics;
using System.Net;

namespace APRP.Services.AuthorityAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorityController : ControllerBase
    {

        private readonly IAuthorityService _authorityService;
 
        private readonly ILogger _logger;
        private readonly IUriService uriService;

        public AuthorityController(APRP.Services.AuthorityAPI.Services.IAuthorityService authorityService, ILogger<AuthorityController> logger,
            APRP.Services.AuthorityAPI.Services.IUriService uriService)
        {
            _authorityService = authorityService;
            _logger = logger;
            this.uriService = uriService;
           
        }

        [HttpGet(Name = "GetAuthorities")]
        [ProducesResponseType(typeof(IEnumerable<Authority>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Authority>>> GetAuthorities([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            AuthorityViewModel authorityViewModel = new AuthorityViewModel();
            //IEnumerable<Authority> authorities = Enumerable.Empty<Authority>();
            _logger.LogInformation("Calling Authority.GetAuthorities");
            var resp = await _authorityService.ListAsync(filter.PageNumber, filter.PageSize).ConfigureAwait(false);
            if (resp.Success)
            {
                var objectResult = (ObjectResult)resp.IActionResult;
                if (objectResult == null) { return BadRequest(authorityViewModel);}
                if (objectResult.StatusCode.HasValue)
                {
                    if (objectResult.StatusCode == (int?)HttpStatusCode.OK)
                    {
                        var result2 = (OkObjectResult)objectResult;
                        if (result2.Value == null) { return BadRequest(authorityViewModel); }
                        authorityViewModel = (AuthorityViewModel)result2.Value;
                        var pagedReponse = PaginationHelper.CreatePagedReponse<Authority>((List<Authority>)authorityViewModel.PagedData, validFilter,
                            authorityViewModel.TotalRecords, uriService, route);
                        return Ok(pagedReponse);
                    }
                }
            }
            return BadRequest(authorityViewModel);

        }

        [Route("GetAuthority/{id}")]
        [HttpGet("{id:length(24)}", Name = "GetAuthority")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(AuthorityResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AuthorityResponse>> GetAuthorityById(int id)
        {
            _logger.LogInformation($"Calling Authority.GetAuthorityById with {id}");
            var authResp = await _authorityService.FindByIdAsync(id);
            if (authResp == null || !authResp.Success)
            {
                _logger.LogError($"Authority with id : {id}, not found.");
                return NotFound(authResp);
            }

            var objectResult = (ObjectResult)authResp.IActionResult;
            if (objectResult == null) { return BadRequest(new AuthorityResponse($"Authority with id : {id}, not found.")); }
            if (objectResult.StatusCode.HasValue)
            {
                if (objectResult.StatusCode == (int?)HttpStatusCode.OK)
                {
                    var result2 = (OkObjectResult)objectResult;
                    if (result2.Value == null) { return BadRequest(new AuthorityResponse($"Authority with id : {id}, not found.")); }
                    var authority  = (Authority)result2.Value;
                    return Ok(new Response<Authority>(authority));
                }
            }
            return BadRequest(new AuthorityResponse($"Authority with id : {id}, not found."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthorityResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AuthorityResponse>> CreateAuthority([FromBody] Authority authority)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var resp = await _authorityService.AddAsync(authority).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return StatusCode(500, "Internal server error");
                }
                else
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult == null) { return BadRequest(new AuthorityResponse($"Authority with name : {authority.Name}, not created.")); }
                    if (objectResult.StatusCode.HasValue)
                    {
                        if (objectResult.StatusCode == (int?)HttpStatusCode.OK)
                        {
                            var result2 = (OkObjectResult)objectResult;
                            if (result2.Value == null) { return BadRequest(new AuthorityResponse($"Authority with name : {authority.Name}, not created.")); }
                            var authority2 = (Authority)result2.Value;
                            return Ok(new AuthorityResponse(authority2));
                        }
                    }
                    return CreatedAtRoute("CreateAuthority", new { id = authority.ID }, authority);
                }
            }
            
        }
    }
}