using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net.Http.Headers;

namespace APRP.Web.Helpers
{
    public class Helper
    {
        public static string RenderViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                    );

                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }

    public class MobileSasaAPI
    {
        private string _apiBaseURI = "https://localhost:44360";

        //production api
        public MobileSasaAPI()
        {

        }

        public HttpClient InitializeClient(string apiBaseURI, string token)
        {
            if (apiBaseURI == null)
            {
                apiBaseURI = _apiBaseURI;
            }
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(/*handler*/);

            //Passing service base url  
            client.BaseAddress = new Uri(apiBaseURI);
            client.DefaultRequestHeaders.Clear();



            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Bearer", token);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;

        }

    }

    public class AfricastingAPI
    {
        private string _apiBaseURI = "https://localhost:44360";

        //production api
        public AfricastingAPI()
        {

        }

        public HttpClient InitializeClient(string apiBaseURI, string token)
        {
            if (apiBaseURI == null)
            {
                apiBaseURI = _apiBaseURI;
            }
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(/*handler*/);

            //Passing service base url  
            client.BaseAddress = new Uri(apiBaseURI);
            client.DefaultRequestHeaders.Clear();



            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("token", token);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("access_token", token);

            return client;

        }

    }

    public class AccessCredentialsAPI
    {
        private string _apiBaseURI = "https://localhost:44360";

        //production api
        public AccessCredentialsAPI()
        {

        }
        public HttpClient InitializeClient(string apiBaseURI, string token)
        {
            if (apiBaseURI == null)
            {
                apiBaseURI = _apiBaseURI;
            }
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(/*handler*/);

            //Passing service base url  
            client.BaseAddress = new Uri(apiBaseURI);
            client.DefaultRequestHeaders.Clear();



            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("token", token);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("access_token", token);

            return client;

        }

    }

    public class APRPAPI
    {
        private string _apiBaseURI = "https://localhost:44360";

        //production api
        public APRPAPI()
        {

        }
        public HttpClient InitializeClient(string apiBaseURI)
        {

            if (apiBaseURI == null)
            {
                apiBaseURI = _apiBaseURI;
            }

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };


            var client = new HttpClient(clientHandler);

            client.BaseAddress = new Uri(apiBaseURI);

            client.DefaultRequestHeaders.Clear();
            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;

        }
    }
    public class TokenAPI
    {
        private string _apiBaseURI = "https://localhost:44360";

        public HttpClient InitializeClient(string apiBaseURI, string token)
        {
            if (apiBaseURI == null)
            {
                apiBaseURI = _apiBaseURI;
            }
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(/*handler*/);

            //Passing service base url  
            client.BaseAddress = new Uri(apiBaseURI);
            client.DefaultRequestHeaders.Clear();



            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;

        }

        internal HttpClient InitializeClient(string url, object p)
        {
            throw new NotImplementedException();
        }
    }

}
