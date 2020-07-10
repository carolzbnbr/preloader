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
            containerRegistry.Register<IRemoteDataService, RemoteDataService>();


            //Register the preloader service which will allow us to consume our preloader
            //implementation through "IPreloaderService" interface from the ViewModels classes.
            containerRegistry.RegisterPreLoaderService();
            //register our preloader implementations, in this case a preloader for loading countries data.
            containerRegistry.RegisterPreLoader<CountriesPreLoader, RestCountriesModel[]>();



        }


    }
}
