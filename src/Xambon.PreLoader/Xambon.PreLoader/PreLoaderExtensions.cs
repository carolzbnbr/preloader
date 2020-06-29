using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Prism.Ioc;

namespace Xambon.PreLoader
{
    public static class PreLoaderExtensions
    {

        private static IContainerProvider containerResolver;
        //private static UnityContainer unityContainer;
        private static System.Collections.Concurrent.ConcurrentDictionary<string, Type> preloadersContainer = new System.Collections.Concurrent.ConcurrentDictionary<string, Type>();

      

        private static IMemoryCache _cache;
        static PreLoaderExtensions()
        {
            containerResolver = Prism.PrismApplicationBase.Current.Container;  // (Prism.PrismApplicationBase.Current as Prism.Unity.PrismApplication).Container;
        }
        private static string GetKey(string name)
        {

            return $"{name}";
        }

        public static IMemoryCache MemoryCache
        {
            get
            {
                return _cache;
            }
        }

        public static void RegisterPreLoaderService(this IContainerRegistry containerRegistry)
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

       

        public static bool RegisterPreLoader<TPreloader>(this IContainerRegistry containerRegistry, string name = null) where TPreloader : IPreLoader
        {
            if (!containerRegistry.IsRegistered<IPreLoaderService>())
            {
                throw new NotSupportedException($"Metodo {nameof(RegisterPreLoader)} não registrado no Prism.");
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


        public static bool RegisterPreLoader<T, TPreloader>(this IContainerRegistry containerRegistry, string name = null) where TPreloader : IPreLoader
        {
            if (!containerRegistry.IsRegistered<IPreLoaderService>())
            {
                throw new NotSupportedException($"Metodo {nameof(RegisterPreLoader)} não registrado no Prism.");
            }

            var type = typeof(T);

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

        public async static Task ForceDataPreLoad(string preloaderName, PreLoadParameters parameters) 
        {
            var key = GetKey(preloaderName);

            if (!preloadersContainer.TryGetValue(preloaderName, out var type))
            {
                throw new NotSupportedException($"Nenhum item {preloaderName} foi encontrado no container. Utilize App.{nameof(RegisterPreLoader)}");
            }

            if (parameters == null)
            {
                parameters = new PreLoadParameters();
            }
            var instance = (IPreLoader)containerResolver.Resolve(type, GetKey(type.FullName));
            if (instance == null)
            {
                throw new NotSupportedException($"{type.FullName} não foi registrado no container. Utilize App.{nameof(RegisterPreLoader)}");
            }

           
            var result = await instance.GetDataAsync(parameters);
            CachePreLoadedResult(preloaderName, result);

        }

        internal static void Remove(string preloaderName)
        {
            var memoryKey = $"{preloaderName}-Memory";
            MemoryCache.Remove(memoryKey);
        }

        internal static void CachePreLoadedResult(string preloaderName, PreLoadResult result)
        {
            if (result != null)
            {
                var memoryKey = $"{preloaderName}-Memory";
                MemoryCache.Set<object>(memoryKey, result.Data, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = result.AbsoluteExpirationRelativeToNow });
            }
        }


        internal static T GetCachedPreLoadedData<T>(string name) where T : class
        {
            var cachedData = GetCachedPreLoadedData(name);
            if (cachedData is T value)
            {
                return value;
            }
            return default(T);
        }

        internal static object GetCachedPreLoadedData(string name)
        {
            var key = GetKey(name);
           

            if (!preloadersContainer.TryGetValue(name, out var type))
            {
                throw new NotSupportedException($"Nenhum item {name} foi encontrado no container. Utilize App.{nameof(RegisterPreLoader)}");
            }

            var memoryKey = $"{name}-Memory";

            var cachedData = MemoryCache.Get<object>(memoryKey);
            return cachedData;
        }


        internal static bool TryGetPreLoadedData<T>(string name, out T value) where T : class
        {
            value = null;
            var cachedData = GetCachedPreLoadedData(name);
            if (cachedData is T valueInternal)
            {
                value = valueInternal;
            }
            return value != null;
        }
    }
}
