using System;
using System.Reactive.Threading.Tasks;
using XamarinPreLoaderSample.Models;
using XamarinPreLoaderSample.Services;
using Xambon.PreLoader;

namespace XamarinPreLoaderSample.PreLoaders
{
    public class CountriesPreLoader: IPreLoader<RestCountriesModel[]>
    {
        private readonly IRemoteDataService remoteDataService;

        public CountriesPreLoader(IRemoteDataService remoteDataService)
        {
            this.remoteDataService = remoteDataService;
        }

        public IObservable<RestCountriesModel[]> GetData(PreLoadParameters parameters)
        {
            var response =  remoteDataService.GetDataAsync().ToObservable();
            return response;
        }

        public TimeSpan GetDataExpiration()
        {
            return TimeSpan.FromMinutes(1);
        }
    }
}
