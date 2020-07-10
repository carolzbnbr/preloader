using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Prism.Ioc;

namespace Xambon.PreLoader
{
   
    public class PreLoaderServiceCore
    {
        private  IContainerProvider containerResolver;
       
        private  System.Collections.Concurrent.ConcurrentDictionary<string, Type> preloadersContainer = new System.Collections.Concurrent.ConcurrentDictionary<string, Type>();
        private  IMemoryCache _cache;
        private static PreLoaderServiceCore _Instance;
        internal static PreLoaderServiceCore Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new PreLoaderServiceCore();
                }
                return _Instance;
            }
        }


        private PreLoaderServiceCore()
        {
           
            containerResolver = Prism.PrismApplicationBase.Current.Container;  
        }




        private  string GetKey(string name)
        {

            return $"{name}";
        }




        internal  void RegisterPreLoaderService(IContainerRegistry containerRegistry)
        {
            if (!containerRegistry.IsRegistered<IMemoryCache>())
            {
                _cache = new MemoryCache(new MemoryCacheOptions() { });
                containerRegistry.RegisterInstance<IMemoryCache>(_cache, "Xambon.PreLoader");
            }

            if (!containerRegistry.IsRegistered<IPreLoaderService>())
            {
                containerRegistry.RegisterInstance<IPreLoaderService>(new PreLoaderService());
            }

        }


        public bool RegisterPreLoader<TPreloader, TResponse>( IContainerRegistry containerRegistry, string name = null) where TPreloader : IPreLoader<TResponse>
        {
            if (!containerRegistry.IsRegistered<IPreLoaderService>())
            {
                throw new NotSupportedException($"Metodo {nameof(RegisterPreLoaderService)}() deve ser registrado no container do Prism.");
            }

            var type = typeof(TPreloader);

            if (name == null)
            {
                name = type.FullName;
            }

            var internalKey = $"{name}-PreLoader";
            if (preloadersContainer.TryAdd(type.Name, typeof(TPreloader)))
            {
                containerRegistry.Register(type, GetKey(type.FullName));
                return true;
            }
            return false;
        }





        internal IObservable<T> InvokePreLoaderWithObservable<T>(string preloaderName, PreLoadParameters parameters)
        {

            var instance = GetPreLoaderInstance<T>(preloaderName);

            string preloadedCacheKey = GetPreLoaderKey(preloaderName, instance, parameters);

            var expiration = instance.GetDataExpiration();


            if (parameters == null)
            {
                parameters = new PreLoadParameters();
            }
            //var observable = instance.GetData(parameters);
            var observable = Observable.Start(() => instance.GetData(parameters), Scheduler.Default)
                .Merge()
                .SubscribeOn(new NewThreadScheduler());

            observable.Subscribe(f =>
            {
                var result = new PreLoadResult(expiration, f);
                SetPreLoadedResult(preloadedCacheKey, result);
            });

            return observable;
        }

        private string GetPreLoaderKey<T>(string preloaderName, IPreLoader<T> instance, PreLoadParameters parameters)
        {
            if (parameters == null)
            {
                parameters = new PreLoadParameters();
            }
            string preloadedCacheKey = preloaderName;
            if (instance is IPreLoaderKey instanceKey)
            {
                preloadedCacheKey = $"{preloaderName}:{instanceKey.GetKey(parameters)}";
            }

            return preloadedCacheKey;
        }


        internal  Task InvokePreLoader<T>(string preloaderName, PreLoadParameters parameters)
        {
            var key = GetKey(preloaderName);
           

            var instance = GetPreLoaderInstance<T>(preloaderName);

            string preloadedCacheKey = GetPreLoaderKey(preloaderName, instance, parameters);
            if (parameters == null)
            {
                parameters = new PreLoadParameters();
            }
            var observable = (IObservable<T>)instance.GetData(parameters);

            var expiration = instance.GetDataExpiration();

            observable.Subscribe(f =>
            {
                var result = new PreLoadResult(expiration, f);
                SetPreLoadedResult(preloadedCacheKey, result);
            });

            //await observable.ForEachAsync(x => Debug.WriteLine(x));
            return Task.CompletedTask;
        }


        private IPreLoader<T> GetPreLoaderInstance<T>(string preloaderName)
        {
            if (!preloadersContainer.TryGetValue(preloaderName, out var type))
            {
                throw new NotSupportedException($"Nenhum item {preloaderName} foi encontrado no container. Utilize App.{nameof(PreLoaderExtensions.RegisterPreLoader)}");
            }
            var instance = (IPreLoader<T>)containerResolver.Resolve(type, GetKey(type.FullName));
            if (instance == null)
            {
                throw new NotSupportedException($"Nenhuma Implementação de \"{preloaderName}\" foi registrado no container. Utilize App.{nameof(PreLoaderExtensions.RegisterPreLoader)}");
            }

            return instance;
        }


        public IMemoryCache MemoryCache
        {
            get
            {
                return _cache;
            }
        }


        internal void RemoveAll()
        {
            foreach (var key in preLoaderCachedKeys)
            {
                MemoryCache.Remove(key);
            }
            preLoaderCachedKeys.Clear();
        }

        internal  void Remove(string preloaderName)
        {
          
            MemoryCache.Remove(preloaderName);
            preLoaderCachedKeys.Remove(preloaderName);
        }
    

        private List<string> preLoaderCachedKeys = new List<string>();

        internal  void SetPreLoadedResult(string preloaderName, PreLoadResult result)
        {
            if (result != null)
            {
               
                preLoaderCachedKeys.Add(preloaderName);
                MemoryCache.Set<object>(preloaderName, result.Data, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = result.AbsoluteExpirationRelativeToNow });
            }
        }

        internal  T GetCachedPreLoadedData<T>(string preloaderName, PreLoadParameters parameters)
        {
          
            var instance = GetPreLoaderInstance<T>(preloaderName);

            string preloadedCacheKey = GetPreLoaderKey(preloaderName, instance, parameters);

            var cachedData = MemoryCache.Get<T>(preloadedCacheKey);
            return cachedData;
        }


        internal  bool TryGetPreLoadedData<T>(string name, PreLoadParameters parameters, out T value)
        {
            value = default(T);
            var cachedData = GetCachedPreLoadedData<T>(name, parameters);

            if (cachedData  != null)
            {
                value = cachedData;
                return true;
            }
            return false;
        }
    }
}
