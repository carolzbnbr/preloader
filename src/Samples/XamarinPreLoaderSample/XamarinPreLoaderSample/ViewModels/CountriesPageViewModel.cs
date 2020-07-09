using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Navigation;
using XamarinPreLoaderSample.Models;
using XamarinPreLoaderSample.PreLoaders;
using XamarinPreLoaderSample.Services;
using XamarinPreLoaderSample.Views;
using Xambon.PreLoader;
using Xambon.PreLoader.Extensions;
namespace XamarinPreLoaderSample.ViewModels
{
    public class CountriesPageViewModel : BindableBase, IAutoInitialize, IInitializeAsync
    {
        private Stopwatch stopWatch;
        public IRemoteDataService RemoteDataService { get; private set; }
        private readonly IPreLoaderService preLoaderService;
         
        public ObservableCollection<RestCountriesModel> _Countries = new ObservableCollection<RestCountriesModel>();
        private bool _usePreLoader;
        private long _ElapsedMilliseconds;

        public CountriesPageViewModel(IRemoteDataService  remoteDataService, IPreLoaderService preLoaderService)
        {
            this.RemoteDataService = remoteDataService;
            this.preLoaderService = preLoaderService;
             stopWatch = new Stopwatch();
        }
        public async  Task InitializeAsync(INavigationParameters parameters)
        {
            if (UsePreLoader)
            {

                await LoadDataWithPreLoader();
            }
            else
            {
                await LoadDataWithoutPreLoaderAsync();
            }
        }

        private async Task LoadDataWithPreLoader()
        {
            stopWatch.Start();

            var response = await preLoaderService.GetOrInvokePreLoaderAsync<RestCountriesModel[]>(nameof(CountriesPreLoader));
            Countries = new ObservableCollection<RestCountriesModel>(response);

            stopWatch.Stop();
            ElapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            Console.WriteLine("Time elapsed: {0}", stopWatch.ElapsedMilliseconds);
        }

        private async Task LoadDataWithoutPreLoaderAsync()
        {
            stopWatch.Start();

            var response = await RemoteDataService.GetDataAsync();
            Countries = new ObservableCollection<RestCountriesModel>(response);

            stopWatch.Stop();
            ElapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            Console.WriteLine("Time elapsed: {0}", stopWatch.ElapsedMilliseconds);
        }

        
        public long ElapsedMilliseconds
        {
            get
            {
                return _ElapsedMilliseconds;
            }

            set
            {
                SetProperty(ref _ElapsedMilliseconds, value);
            }
        }
        public bool UsePreLoader
        {
            get
            {
                return _usePreLoader;
            }

            set
            {
                SetProperty(ref _usePreLoader, value);
            }
        }
        public ObservableCollection<RestCountriesModel> Countries
        {
            get
            {
                return _Countries;
            }

            set
            {
                SetProperty(ref _Countries, value);
            }
        }
    }
}
