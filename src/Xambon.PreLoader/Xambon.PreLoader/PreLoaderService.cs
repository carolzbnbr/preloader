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
        }


        public T GetPreLoadedData<T>(string name, PreLoadParameters parameters = null)
        {
            return PreLoaderCore.Instance.GetCachedPreLoadedData<T>(name, parameters);
        }

        public bool TryGetPreLoadedData<T>(string name, out T value, PreLoadParameters parameters = null)
        {
            T valueInternal;
            var result = PreLoaderCore.Instance.TryGetPreLoadedData<T>(name, parameters, out valueInternal);

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
