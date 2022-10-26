using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using APRP.Web.ViewModels.COSTES;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using APRP.Web.Helpers;

namespace APRP.Web.Persistence.Repositories
{
    public class COSTESRepository : BaseRepository, ICOSTESRespository
    {
        public IConfiguration Configuration { get; }

        public COSTESRepository(AppDbContext context, IConfiguration configuration) : base(context)
        {
            Configuration = configuration;
        }
        public async Task<IActionResult> GetInstructutedWorkItemsAsync(string Token,  int Year, string RegionCode)
        {
            InstructutedWorkItemsParamsViewModel instructutedWorkItemsParams = new InstructutedWorkItemsParamsViewModel();
            instructutedWorkItemsParams.Year = Year;
            instructutedWorkItemsParams.RegionCode = RegionCode;
            TokenAPI tokenAPI = new TokenAPI();
            string url = Configuration["COSTESTURI"];
            HttpClient client = tokenAPI.InitializeClient(url, Token);
            var content = new StringContent(JsonConvert.SerializeObject(instructutedWorkItemsParams), Encoding.UTF8, "application/json");
            HttpResponseMessage res = client.PostAsync("api/V1/InstructedWorkItems", content).Result;

            if (res.IsSuccessStatusCode)
            {
                var cnt = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject instructedWorksObj = JObject.Parse(cnt);
                var test = instructedWorksObj["itemlist"];
                //IList<GetInstructutedWorkItemsViewModel> getInstructutedWorkItemsViewModels =
                //    (IList<GetInstructutedWorkItemsViewModel>)test;

                IList<GetInstructutedWorkItemsViewModel> getInstructutedWorkItemsViewModels
                    = JsonConvert.DeserializeObject<IList<GetInstructutedWorkItemsViewModel>>(test.ToString());
                return Ok(getInstructutedWorkItemsViewModels);
            }
            else
            {
                return BadRequest(Enumerable.Empty<GetInstructutedWorkItemsViewModel>());
            }

        }

        public Task<IActionResult> ListTokensAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> EncodeTo64Async(string toEncode)
        {
            byte[] toEncodeAsBytes = null;
            if (toEncode == null)
            {
                string COSTESUsername = Configuration["COSTESUsername"];
                string COSTESSecretKey = Configuration["COSTESSecretKey"];
                toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes($"{COSTESUsername}:{COSTESSecretKey}");
            }
            else
            {
                toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            }

            string returnValue= null;
            returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            if (toEncodeAsBytes == null)
            {
                return BadRequest(returnValue);
            }else
            {
                return Ok(returnValue);
            }
           
        }

        public async Task<IActionResult> GetAccessTokenAsync(string Token)
        {
            //InstructutedWorkItemsParamsViewModel instructutedWorkItemsParams = new InstructutedWorkItemsParamsViewModel();
            //instructutedWorkItemsParams.Year = 2019;
            //instructutedWorkItemsParams.RegionCode = "1";
            AccessCredentialsAPI accessCredentialsAPI = new AccessCredentialsAPI();
            string url = Configuration["COSTESTURI"];
            HttpClient client = accessCredentialsAPI.InitializeClient(url, Token);
            //var content = new StringContent(JsonConvert.SerializeObject(instructutedWorkItemsParams), Encoding.UTF8, "application/json");
            HttpResponseMessage res = client.PostAsync("api/V1/GetAccessToken", null).Result;

            if (res.IsSuccessStatusCode)
            {
                var cnt = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject instructedWorksObj = JObject.Parse(cnt);
                var access_token = instructedWorksObj["access_token"];
                return Ok(access_token.ToString());
            }
            else
            {
                return BadRequest(Enumerable.Empty<GetInstructutedWorkItemsViewModel>());
            }
        }
    }
}
