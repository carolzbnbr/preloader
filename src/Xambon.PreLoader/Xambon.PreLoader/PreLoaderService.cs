using System.Threading.Tasks;

namespace Xambon.PreLoader
{
    public class PreLoaderService : IPreLoaderService
    {
        public Task InvokePreLoader(string name, PreLoadParameters parameters = null)
        {
            return PreLoaderExtensions.ForceDataPreLoad(name, parameters);
        }

        public void SetPreLoadResult(string name, PreLoadResult result)
        {
             PreLoaderExtensions.CachePreLoadedResult(name, result);
        }

        public T GetPreLoadedData<T>(string name) where T : class
        {
            return PreLoaderExtensions.GetCachedPreLoadedData<T>(name);
        }

        public object GetPreLoadedData(string name)
        {
            return PreLoaderExtensions.GetCachedPreLoadedData(name);
        }

        public bool TryGetPreLoadedData<T>(string name, out T value) where T : class
        {
            T valueInternal;
            var result = PreLoaderExtensions.TryGetPreLoadedData<T>(name, out valueInternal);

            value = valueInternal;
            return result;
        }

        public void Remove(string preloaderName)
        {
             PreLoaderExtensions.Remove(preloaderName);
        }

    }
}
