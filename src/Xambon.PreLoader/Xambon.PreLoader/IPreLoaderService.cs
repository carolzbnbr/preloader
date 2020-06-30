using System;
using System.Threading.Tasks;

namespace Xambon.PreLoader
{
    public interface IPreLoaderService
    {
        
        ///// <summary>
        ///// Força o carregamento prévio dos dados da PreLoader informada.
        ///// </summary>
        ///// <param name="preloaderName"></param>
        //Task InvokePreLoader(string preloaderName, PreLoadParameters parameters = null);

        void InvokePreLoader<T>(string preloaderName, PreLoadParameters parameters = null);

        /// <summary>
        /// Tenta retornar os dados da ultima execucao do PreLoader, caso haja um.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="preloaderName"></param>
        /// <returns></returns>
        bool TryGetPreLoadedData<T>(string preloaderName, out T value);

        /// <summary>
        /// Retorna os dados da ultima execucao do PreLoader, caso haja um.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="preloaderName"></param>
        /// <returns></returns>
        T GetPreLoadedData<T>(string preloaderName);

        /// <summary>
        /// Remove os dados resultantes da ultima invocacao do PreLoader do cache, caso ainda nao tenham sido expirado.
        /// </summary>
        /// <param name="preloaderName"></param>
        void Remove(string preloaderName);

        /// <summary>
        /// Remove do gerenciador de preloaders todos os dados em cache das invocacoes de PreLoaders que ainda estão na memoria.
        /// </summary>
        void RemoveAll();
    }
}
