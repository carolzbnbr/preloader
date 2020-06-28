

# Xambon.PreLoader for Xamarin Forms

## As the name implies, it helps you to pre load data used by your xamarin forms app page gets called.


## What does that mean?
In a regular Xamarin Forms application, every time a page is loaded (navigated to), some data needs to be loaded from a remote source, or even from a disk, to fill up Xaml's Views.

Even if you cache the data to disk, either by implementing your own caching strategy or by using [Akavache](https://github.com/reactiveui/Akavache/) (which by the way is great), your data will need to be deserialized back to it's POCO original class so you can consume it on your viewmodels.

We usually load the data thru **View Mode's constructor**, **OnAppearing method**  or on the **OnNavigatedTo method**.

The problem relays on the mentioned methods above as it's very expensive to get and deserialize data on then which in turn hang page loads.

What *PreLoader* does is to load/deserialize the data before a page navigation method is called and maintain it in memory for a short period. In other words, it should load data before the a **NavigationService.Navigate()** method called.

## Getting Started


Xambon.PreLoader depends on PRISM for Dependency Injection, so first thing we need to do is to register the **RegisterPreLoaderService** along with our own PreLoaders on the App.cs class of our shared project:

```csharp
 protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    ...
    containerRegistry.RegisterPreLoaderService();
    containerRegistry.RegisterPreLoader<EmployeesPreLoader>();
    containerRegistry.RegisterPreLoader<OtherPreLoader1>();
    containerRegistry.RegisterPreLoader<OtherPreLoader2>();
    containerRegistry.RegisterPreLoader<OtherPreLoader3>();
    ...
}
```

A *PreLoader* is a class that we must implement for every data we need and is responsible for loading our data a disk cache or remote api. For every data retrived it will be kept **as is** aka **deserialized**, to a memory cache.

In order to do that, we need to implment the class **IPreLoader** like as follow:

```csharp
 public class EmployeesPreLoader : IPreLoader
    {
       ///http://dummy.restapiexample.com/api/v1/employees
    
        private readonly IEmployeeService employeeService;

        public EmployeesPreLoader(IEmployeeService  employeeService)
        {
            this.employeeService = employeeService;
        }

        public async Task<PreLoadResult> GetDataAsync(PreLoadParameters parameters)
        {
            var response = await employeeService.GetRemoteEmployees(true, true, false, true, cancellationTokenSource.Token);

            return new PreLoadResult(TimeSpan.FromMinutes(3), response);
        }
    }
```



