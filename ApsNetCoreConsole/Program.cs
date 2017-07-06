using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApsNetCoreConsole
{
    class Program
    {        
        static void Main(string[] args)
        {            
            try
            {
                // async console app:
                // https://stackoverflow.com/a/38127525/54159
                MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

                var pingResult = await ExecuteGetPing(client);
                var adUserInfo = await ExecuteGetAdInfo(client);
                var adUserInfoList = await ExecuteGetAdInfoList(client);
            }
        }

        private static async Task<PingResult> ExecuteGetPing(HttpClient client)
        {
            //----------------------
            // GET api/adinfo/ping             
            var response = await client.GetAsync("api/adinfo/ping");
            if (response.IsSuccessStatusCode)
            {
                var stringData = await response.Content.ReadAsStringAsync();

                // convert json data to c# object:
                var pingResult = JsonConvert.DeserializeObject<PingResult>(stringData);

                // output c# object to console
                Console.WriteLine($"pingResult = {JsonConvert.SerializeObject(pingResult)}");

                return pingResult;
            }
            else
            {
                throw new Exception("some stupid error during http request");
            }            
        }

        private static async Task<AdUserInfo> ExecuteGetAdInfo(HttpClient client)
        {
            //----------------------
            // Current User
            // GET api/adinfo             
            var response = await client.GetAsync("api/adinfo");
            if (response.IsSuccessStatusCode)
            {
                var stringData = await response.Content.ReadAsStringAsync();

                // use "data" node to convert to c# object:
                // https://stackoverflow.com/a/29702862/54159
                var parsed = JObject.Parse(stringData);
                var data = parsed["data"];

                // convert json data to c# object:
                var adUserInfo = JsonConvert.DeserializeObject<AdUserInfo>(data.ToString());

                // output c# object to console
                Console.WriteLine($"adUserInfo = {JsonConvert.SerializeObject(adUserInfo)}");

                return adUserInfo;
            }
            else
            {
                throw new Exception("some stupid error during http request");
            }
        }

        private static async Task<List<AdUserInfoListItem>> ExecuteGetAdInfoList(HttpClient client)
        {
            //----------------------
            // Get company part user list
            // GET api/adinfo/list/ensi
            var response = await client.GetAsync("api/adinfo/list/ensi");
            if (response.IsSuccessStatusCode)
            {
                var stringData = await response.Content.ReadAsStringAsync();

                // use "data" node to convert to c# object:
                // https://stackoverflow.com/a/29702862/54159
                var parsed = JObject.Parse(stringData);
                var data = parsed["data"];

                // convert json data to c# object:
                var adUserInfoListItems = JsonConvert.DeserializeObject<List<AdUserInfoListItem>>(data.ToString());

                // output kurzzeichen to console
                Console.WriteLine($"adUserInfoList = [{adUserInfoListItems.Count}]: {string.Join(", ", adUserInfoListItems.Select(x => x.KurzZeichen))}");

                return adUserInfoListItems;
            }
            else
            {
                throw new Exception("some stupid error during http request");
            }
        }
    }
}