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
   
    public class PreLoaderCore
    {
        private  IContainerProvider containerResolver;
       
        private  System.Collections.Concurrent.ConcurrentDictionary<string, Type> preloadersContainer = new System.Collections.Concurrent.ConcurrentDictionary<string, Type>();
        private  IMemoryCache _cache;
        private static PreLoaderCore _Instance;
        internal static PreLoaderCore Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new PreLoaderCore();
                }
                return _Instance;
            }
        }


        private PreLoaderCore()
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
            var key = GetKey(preloaderName);

            if (!preloadersContainer.TryGetValue(preloaderName, out var type))
            {
                throw new NotSupportedException($"{preloaderName} foi encontrado no container. Crie um novo PreLoader e Registre-o usando o comando {nameof(PreLoaderExtensions.RegisterPreLoader)}(\"{preloaderName}\") na classe App.Cs");
            }

            if (parameters == null)
            {
                parameters = new PreLoadParameters();
            }
            var instance = (IPreLoader<T>)containerResolver.Resolve(type, GetKey(type.FullName));
            if (instance == null)
            {
                throw new NotSupportedException($"{type.FullName} não foi registrado no container. Crie um novo PreLoader e Registre-o usando o comando {nameof(PreLoaderExtensions.RegisterPreLoader)}(\"{preloaderName}\") na classe App.Cs ");
            }

            var expiration = instance.GetDataExpiration();
            var observable = Observable.Start(() => instance.GetData(parameters), Scheduler.Default)
                .Merge()
                //.ObserveOnDispatcher()
                .SubscribeOn(new NewThreadScheduler());

            observable.Subscribe(f =>
            {
                var result = new PreLoadResult(expiration, f);
                SetPreLoadedResult(preloaderName, result);
            });

            return observable;
        }

        internal  Task InvokePreLoader<T>(string preloaderName, PreLoadParameters parameters)
        {
            var key = GetKey(preloaderName);

            if (!preloadersContainer.TryGetValue(preloaderName, out var type))
            {
                throw new NotSupportedException($"Nenhum item {preloaderName} foi encontrado no container. Utilize App.{nameof(PreLoaderExtensions.RegisterPreLoader)}");
            }

            if (parameters == null)
            {
                parameters = new PreLoadParameters();
            }
            var instance = (IPreLoader<T>)containerResolver.Resolve(type, GetKey(type.FullName));
            if (instance == null)
            {
                throw new NotSupportedException($"Nenhuma Implementação de \"{preloaderName}\" foi registrado no container. Utilize App.{nameof(PreLoaderExtensions.RegisterPreLoader)}");
            }

            var observable = (IObservable<T>)instance.GetData(parameters);

            var expiration = instance.GetDataExpiration();

            observable.Subscribe(f =>
            {
                var result = new PreLoadResult(expiration, f);
                SetPreLoadedResult(preloaderName, result);
            });

            //await observable.ForEachAsync(x => Debug.WriteLine(x));
            return Task.CompletedTask;
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
            var memoryKey = $"{preloaderName}-Memory";
            MemoryCache.Remove(memoryKey);
            preLoaderCachedKeys.Remove(memoryKey);
        }


        private List<string> preLoaderCachedKeys = new List<string>();

        internal  void SetPreLoadedResult(string preloaderName, PreLoadResult result)
        {
            if (result != null)
            {
                var memoryKey = $"{preloaderName}-Memory";
                preLoaderCachedKeys.Add(memoryKey);
                MemoryCache.Set<object>(memoryKey, result.Data, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = result.AbsoluteExpirationRelativeToNow });
            }
        }

     
        internal  T GetCachedPreLoadedData<T>(string name)
        {
            var key = GetKey(name);

            var memoryKey = $"{name}-Memory";

            var cachedData = MemoryCache.Get<T>(memoryKey);
            return cachedData;
        }


        internal  bool TryGetPreLoadedData<T>(string name, out T value)
        {
            value = default(T);
            var cachedData = GetCachedPreLoadedData<T>(name);

            if (cachedData  != null)
            {
                value = cachedData;
                return true;
            }
            return false;
        }
    }
}
