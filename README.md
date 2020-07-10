

# Xambon.PreLoader for Xamarin Forms

## As the name implies, it helps you to pre load data used by your xamarin forms app page gets called.

## Sample App - Regular Page Load versus PreLoaded

![Sample App](https://github.com/carolzbnbr/preloader/blob/master/Images/screencast.gif)


## What does that mean?
In a regular Xamarin Forms application, every time a page is loaded (navigated to), some data needs to be loaded from a remote source, or even from a disk, to fill up Xaml's Views.

Even if you cache the data to disk, either by implementing your own caching strategy or by using [Akavache](https://github.com/reactiveui/Akavache/) (which by the way is great), your data will need to be deserialized back to it's POCO original class so you can consume it on your viewmodels.

We usually load the data thru **View Mode's constructor**, or in methods like **Initialize**, **InitializeAsync**, **OnAppearing**, or **OnNavigatedTo**, of our viewmodels.

The problem relays on the mentioned methods above as it's very expensive to get and deserialize data on then which in turn hang page loads. Also, if your remote rest service is slow, our user experience may also be affected whithout a solution like PreLoader.

What *PreLoader* does is to load/deserialize the data before a page navigation method is called and maintain it in memory for a short period. In other words, it should load data before the a **NavigationService.Navigate()** method called.



## nuget Package
You can add this component to your project by installing/downloading a nuget package  **Xambon.PreLoader** in the nuget package gallery of visual studio.


## Getting Started
Xambon.PreLoader depends on PRISM for Dependency Injection, so first thing we need to do is to register the **RegisterPreLoaderService** along with our PreLoader implementation, in the App.cs class of our shared project:

```csharp
 protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    ...
    containerRegistry.RegisterPreLoaderService();
    containerRegistry.RegisterPreLoader<CountriesPreLoader>();

}
```

A *PreLoader* is a class that we must implement for every data we need to retrieve and is responsible for loading our data to a memory cache. For every data retrived it will be kept **as is** aka **deserialized**, to a memory cache.

## Implementing a PreLoader
In order to cache the data we need to instruct preLoader service on how to acquire your data and how long it should be kept in memory. 

To do that, we need to implement the class **IPreLoader<TResponse>** as follow:

```csharp
  public class CountriesPreLoader: IPreLoader<RestCountriesModel[]>
    {
        private readonly IRemoteDataService remoteDataService;

        public CountriesPreLoader(IRemoteDataService remoteDataService)
        {
            this.remoteDataService = remoteDataService;
        }

        public IObservable<RestCountriesModel[]> GetData(PreLoadParameters parameters)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://restcountries.eu");
            var clientResponse = await client.GetAsync("rest/v2/all");

            var json = await clientResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<RestCountriesModel[]>(json);
            
            return response.ToObservable();
        }

        public TimeSpan GetDataExpiration()
        {
            return TimeSpan.FromMinutes(1);
        }
    }
```

The generic type parameter **"TResponse"** corresponds to the data that our preloader implementation should return and kept in the preloader service cache.


## Invoking the preloader
One of the gotchas of preloader, as the name implies, is to Pre Load the data before a page navigation event occurs. 

We do that, by calling **IPreLoaderService.InvokePreLoader()** so that it can invoke our preloader on another thread and keep data in memory for a short enough period to garantee that when the page navigation event is invoked, we already have the data in memory to consume it. 

For instance:

```csharp
 public class MainPageViewModel: BindableBase, IInitialize
   {
      public void Initialize(INavigationParameters parameters)
      {
          PreLoaderService.InvokePreLoader<RestCountriesModel[]>(nameof(CountriesPreLoader));
      }
   }    
```

In the case above, we implemented a prism's interface called **IInitialize** on our view models, and also received in the constructor the **IPreLoaderService** instance.


## Getting the preloaded data
After we invoked the preloader on a "parent page" (a page that initiated the navitation to the page that will be consuming the pre loaded data), we need to get the actual data in order to use it and populate our views, we do that by calling **preLoaderService.GetOrInvokePreLoaderAsync** 

For instance:

```csharp
    public class CountriesPageViewModel : BindableBase, IAutoInitialize, IInitializeAsync
    {
       public ObservableCollection<RestCountriesModel> _Countries = new ObservableCollection<RestCountriesModel>();

       public async  Task InitializeAsync(INavigationParameters parameters)
       {
           var response = await preLoaderService.GetOrInvokePreLoaderAsync<RestCountriesModel[]>(nameof(CountriesPreLoader));
           Countries = new ObservableCollection<RestCountriesModel>(response);
       }
    }
```

We also have an extension methods for reactive ui, so we can get the data along with [Akavache](https://github.com/reactiveui/Akavache/) making the component even more powerful if we use the Akavache's extensions methods like **GetAndFetchLatest** and **GetOrCreateObject**. 


## Sample code
Please take a look at the Xamarin Forms Sample Code for more details. 


