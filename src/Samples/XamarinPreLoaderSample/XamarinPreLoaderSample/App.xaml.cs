using System;
using System.Collections.Generic;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Unity;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using XamarinPreLoaderSample.Models;
using XamarinPreLoaderSample.PreLoaders;
using XamarinPreLoaderSample.Services;
using XamarinPreLoaderSample.ViewModels;
using XamarinPreLoaderSample.Views;
using Xambon.PreLoader;

namespace XamarinPreLoaderSample
{
    [Preserve(AllMembers = true)]
    public partial class App : PrismApplication
    {

   
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected async override void OnInitialized()
        {
            
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(Views.MainPage)}");
            
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<CountriesPage>();

            containerRegistry.RegisterPreLoaderService();
            containerRegistry.RegisterPreLoader<CountriesPreLoader, RestCountriesModel[]>();

            containerRegistry.Register<IRemoteDataService, RemoteDataService>();


        }


    }
}
