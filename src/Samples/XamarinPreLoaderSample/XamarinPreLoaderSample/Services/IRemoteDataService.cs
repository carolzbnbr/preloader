using System.Threading.Tasks;
using XamarinPreLoaderSample.Models;

namespace XamarinPreLoaderSample.Services
{
    public interface IRemoteDataService
    {
        string RemoteUrl { get; set; }
        Task<RestCountriesModel[]> GetDataAsync();
    }
}
