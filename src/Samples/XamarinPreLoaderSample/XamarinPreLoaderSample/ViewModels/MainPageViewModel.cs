using System;
using System.Diagnostics;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using XamarinPreLoaderSample.Models;
using XamarinPreLoaderSample.PreLoaders;
using XamarinPreLoaderSample.Views;
using Xambon.PreLoader;

namespace XamarinPreLoaderSample.ViewModels
{
    public class MainPageViewModel: BindableBase, IInitialize
    {
        private  IPreLoaderService PreLoaderService { get;  set; }
        public INavigationService NavigationService { get; private set; }
        public DelegateCommand<object> RegularPageCommand { get; private set; }
        public DelegateCommand<object> PreloadedPageCommand { get; private set; }


        public MainPageViewModel(INavigationService navigationService, IPreLoaderService preLoaderService)
        {
            this.NavigationService = navigationService;
            this.PreLoaderService = preLoaderService;
            RegularPageCommand = new DelegateCommand<object>(OnRegularPageCommand_Tapped);
            PreloadedPageCommand = new DelegateCommand<object>(OnPreloadedPageCommand_Tapped);
        }


        public void Initialize(INavigationParameters parameters)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            //preloads data to memory
            PreLoaderService.InvokePreLoader<RestCountriesModel[]>(nameof(CountriesPreLoader));

            Console.WriteLine("MainPageViewModel.Initialize() - Time elapsed: {0}", stopWatch.ElapsedMilliseconds);
        }


        private async void OnPreloadedPageCommand_Tapped(object obj)
        {
            var nav = new NavigationParameters
            {
                { "UsePreLoader", true }
            };
            await NavigationService.NavigateAsync($"{nameof(CountriesPage)}", nav);
        }

        private async void OnRegularPageCommand_Tapped(object obj)
        {
            await NavigationService.NavigateAsync($"{nameof(CountriesPage)}");
        }
    }
}
