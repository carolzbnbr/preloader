using System;
using System.Threading.Tasks;

namespace Xambon.PreLoader
{
    public interface IPreLoaderService
    {
        /// <summary>
        /// Força o carregamento prévio dos dados da PreLoader informada.
        /// </summary>
        /// <param name="preloaderName"></param>
        Task InvokePreLoader(string preloaderName, PreLoadParameters parameters = null);

        /// <summary>
        /// Seta os dados de resultados do Preload. 
        /// </summary>
        /// <param name="preloaderName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        void SetPreLoadResult(string preloaderName, PreLoadResult result);

        /// <summary>
        /// Retorna os dados, caso existam, da preloader informada.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="preloaderName"></param>
        /// <returns></returns>
        T GetPreLoadedData<T>(string preloaderName) where T : class;



        /// <summary>
        /// Tenta retornar os dados, caso existam, da preloader informada.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="preloaderName"></param>
        /// <returns></returns>
        bool TryGetPreLoadedData<T>(string preloaderName, out T value) where T : class;

        /// <summary>
        /// Tenta retornar os dados, caso existam, da preloader informada.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="preloaderName"></param>
        /// <returns></returns>
        object GetPreLoadedData(string preloaderName);

        /// <summary>
        /// Remove os dados em cache do preloader informado.
        /// </summary>
        /// <param name="preloaderName"></param>
        void Remove(string preloaderName);
    }
}
