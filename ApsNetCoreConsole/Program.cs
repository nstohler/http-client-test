using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApsNetCoreConsole
{
    class Program
    {
        // async console app:
        // https://stackoverflow.com/a/38127525/54159

        static void Main(string[] args)
        {            
            try
            {
                MainAsync(args).GetAwaiter().GetResult();

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            Console.WriteLine("done.");
            Console.ReadLine();
        }

        public static async Task MainAsync(string[] args)
        {
            // Windows authentication for HttpClient:
            // add "new HttpClientHandler() { UseDefaultCredentials = true })" to ctor call

            using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))            
            {
                // http://www.binaryintellect.net/articles/6903087e-5e5c-445a-be20-cee0a7eb434e.aspx                
                client.BaseAddress = new Uri("https://e200681-web01.ensi.ch:44301/");
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);

                //----------------------
                // GET api/adinfo/ping             
                var response = await client.GetAsync("api/adinfo/ping");
                if (response.IsSuccessStatusCode)
                {
                    var stringData = await response.Content.ReadAsStringAsync();
                    var pingResult = JsonConvert.DeserializeObject<PingResult>(stringData);
                    
                    Console.WriteLine($"pingResult = {JsonConvert.SerializeObject(pingResult)}");
                }
                else
                {
                    throw new Exception("some stupid error during http request");
                }

                //----------------------
                // GET api/adinfo             
                response = await client.GetAsync("api/adinfo");
                if (response.IsSuccessStatusCode)
                {
                    var stringData = await response.Content.ReadAsStringAsync();

                    // use "data" node to convert to cs object:
                    // https://stackoverflow.com/a/29702862/54159
                    var parsed = JObject.Parse(stringData);
                    var data = parsed["data"];

                    var adUserInfo = JsonConvert.DeserializeObject<AdUserInfo>(data.ToString());

                    Console.WriteLine($"adUserInfo = {JsonConvert.SerializeObject(adUserInfo)}");
                }
                else
                {
                    throw new Exception("some stupid error during http request");
                }
            }
        }
    }
}