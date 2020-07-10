using System;
using System.Threading.Tasks;

namespace Xambon.PreLoader
{
    /// <summary>
    /// Permite ao imementador de <see cref="IPreLoader{TResponse}" definir o nome da chave no cache. Caso contrário o nome utilizado na chave será o nome do proprio preloader./>
    /// </summary>
    public interface IPreLoaderKey
    {
        /// <summary>
        /// Retorna uma chave alternativa para os dados retornados por <see cref="IPreLoader<TResponse>.GetData(PreLoadParameters)"/>.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string GetKey(PreLoadParameters parameters);
    }


    /// <summary>
    /// Every data that needs to be cache in a preloader service must implement this interface. 
    /// </summary>
    /// <typeparam name="TResponse">Corresponds to the returning type of the your data that this preloader returns</typeparam>
    public interface IPreLoader<TResponse> 
    {
        /// <summary>
        /// How long your data returned by <see cref="GetData(PreLoadParameters)"/> should be keep alive in the preloader service cache.
        /// It is recomended a short interval, like one to a couple of minutes.
        /// </summary>
        /// <returns></returns>
        TimeSpan GetDataExpiration();

        /// <summary>
        /// Gets the actual data that will be cached in the preloader service. 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IObservable<TResponse> GetData(PreLoadParameters parameters);
    }
}
