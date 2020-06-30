using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xambon.PreLoader
{
    public class PreLoaderService : IPreLoaderService
    {

        public void InvokePreLoader<T>(string preloaderName, PreLoadParameters parameters = null)
        {
            var observable = PreLoaderCore.Instance.InvokePreLoaderWithObservable<T>(preloaderName, parameters);
           // observable.Subscribe(f => Debug.WriteLine($"Invoked {nameof(PreLoaderCore)}.{nameof(PreLoaderCore.InvokePreLoaderWithObservable)}"));
        }


        public T GetPreLoadedData<T>(string name)
        {
            return PreLoaderCore.Instance.GetCachedPreLoadedData<T>(name);
        }

        public bool TryGetPreLoadedData<T>(string name, out T value)
        {
            T valueInternal;
            var result = PreLoaderCore.Instance.TryGetPreLoadedData<T>(name, out valueInternal);

            value = valueInternal;
            return result;
        }

        /// <summary>
        /// Remove os dados resultantes da ultima invocacao do PreLoader do cache, caso ainda nao tenham sido expirado.
        /// </summary>
        /// <param name="preloaderName"></param>
        public void Remove(string preloaderName)
        {
            PreLoaderCore.Instance.Remove(preloaderName);
        }

        /// <summary>
        /// Remove do gerenciador de preloaders todos os itens em cache atualmente.
        /// </summary>
        public void RemoveAll()
        {
            PreLoaderCore.Instance.RemoveAll();
        }
        


    }
}
