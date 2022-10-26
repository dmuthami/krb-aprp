using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using APRP.Web.ViewModels.DTO;
using APRP.Web.Helpers;

namespace APRP.Web.Persistence.Repositories
{
    public class AfricastingRepository : BaseRepository, IAfricastingRepository
    {
        public IConfiguration Configuration { get; }

        public AfricastingRepository(AppDbContext context, IConfiguration configuration) : base(context)
        {
            Configuration = configuration;
        }
        public async Task<IActionResult> SendSMSToAfricasting(Africasting africasting)
        {
            //check if africasting is true and send
            bool useAfricasting = false;
            bool resultAfricasting = bool.TryParse(Configuration.GetSection("AfricastingSettings")["SendSMS"], out useAfricasting);
            if (useAfricasting == true)
            {
                //Remove + in the mobile number
                var no = africasting.sendto.Split("+");
                africasting.sendto = no[1];

                TokenAPI tokenAPI = new TokenAPI();
                string url = Configuration.GetSection("AfricastingSettings")["AfricastingURL"];
                HttpClient client = tokenAPI.InitializeClient(url, null);
                var content = new StringContent(JsonConvert.SerializeObject(africasting), Encoding.UTF8, "application/json");
                HttpResponseMessage res = client.PostAsync("/api/SmsqueueApi/addsmsqueue", content).Result;

                if (res.IsSuccessStatusCode)
                {
                    var cnt = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JObject instructedWorksObj = JObject.Parse(cnt);
                    var test = instructedWorksObj["msgcode"];
                    return Ok(test);
                }
                else
                {
                    return BadRequest(res.StatusCode);
                }
            } else
            {
                return BadRequest("useAfricasting = false");
            }
        }

        public async Task<IActionResult> SendSMSViaMobileSasa(MobileSasa mobileSasa)
        {
            //check if africasting is true and send
            bool usemobileSasa = false;
            bool resultAfricasting = bool.TryParse(Configuration.GetSection("MobileSasaSettings")["SendSMS"], out usemobileSasa);
            if (usemobileSasa == true)
            {
                MobileSasaAPI mobileSasaAPI = new MobileSasaAPI();
                string url = Configuration.GetSection("MobileSasaSettings")["PostSMSURL"];
                HttpClient client = mobileSasaAPI.InitializeClient(url, Configuration.GetSection("MobileSasaSettings")["access_token"]);
                var content = new StringContent(JsonConvert.SerializeObject(mobileSasa), Encoding.UTF8, "application/json");
                HttpResponseMessage res = client.PostAsync("api/post-sms", content).Result;

                if (res.IsSuccessStatusCode)
                {
                    var cnt = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JObject mobileSasaResponseObj = JObject.Parse(cnt);
                    var test = mobileSasaResponseObj["status"];
                    return Ok(test);
                }
                else
                {
                    return BadRequest(res.StatusCode);
                }
            }
            else
            {
                return BadRequest("useAfricasting = false");
            }
        }
    }
}
