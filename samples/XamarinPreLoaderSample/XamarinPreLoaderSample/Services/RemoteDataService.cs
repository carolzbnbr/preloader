using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XamarinPreLoaderSample.Models;

namespace XamarinPreLoaderSample.Services
{
    public class RemoteDataService : IRemoteDataService
    {
        private static HttpClient client;

        public RemoteDataService()
        {
            RemoteUrl = "https://restcountries.eu/rest/v2/all";
        }

        public string RemoteUrl { get; set; }

        public async Task<RestCountriesModel[]> GetDataAsync()
        {
            if (client == null)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("https://restcountries.eu");

            }
            var clientResponse = await client.GetAsync("rest/v2/all");

            var json = await clientResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<RestCountriesModel[]>(json);
            return response;
        }
    }
}
