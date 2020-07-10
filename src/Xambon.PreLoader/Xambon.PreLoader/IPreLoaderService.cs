using System;
using System.Threading.Tasks;

namespace Xambon.PreLoader
{
    public interface IPreLoaderService
    {

        /// <summary>
        /// Invokes the PreLoader so that the data returned by your preloader are stored in a local memory cache.
        /// </summary>
        /// <typeparam name="TResponse">Corresponds to the returning type of the your data that your preloader implements</typeparam>
        /// <param name="preloaderName">An alias of your preloader implementation you want to invoke</param>
        /// <param name="parameters">optional parameters your preloader should receive</param>
        /// <remarks>
        /// The invocaton process initiates your preloader and imediate return the control back to your code. The actual preloader execution is done on another thread.
        /// </remarks>
        void InvokePreLoader<TResponse>(string preloaderName, PreLoadParameters parameters = null);

        /// <summary>
        /// Attempts to return the PreLoader's data from the cache, in case it exists.
        /// </summary>
        /// <typeparam name="TResponse">Corresponds to the returning type of the your data that your preloader implements</typeparam>
        /// <param name="preloaderName">An alias of your preloader implementation you want to get data</param>
        /// <returns></returns>
        bool TryGetPreLoadedData<TResponse>(string preloaderName, out TResponse value, PreLoadParameters parameters = null);

        /// <summary>
        /// Retorna os dados da ultima execucao do PreLoader informado, caso haja um.
        /// </summary>
        /// <typeparam name="TResponse">Corresponds to the returning type of the your data that your preloader implements</typeparam>
        /// <param name="preloaderName">An alias of your preloader implementation you want to get data from</param>
        /// <returns></returns>
        TResponse GetPreLoadedData<TResponse>(string preloaderName, PreLoadParameters parameters = null);

        /// <summary>
        /// Remove os dados resultantes da ultima invocacao do PreLoader do cache, caso ainda nao tenham sido expirados.
        /// </summary>
        /// <param name="preloaderName">An alias of your preloader implementation you want to remove from cache</param>
        void Remove(string preloaderName);

        /// <summary>
        /// Remove from the preloader service all Preloader's cached data.
        /// </summary>
        void RemoveAll();
    }
}
