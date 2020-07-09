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

    public interface IPreLoader<TResponse> 
    {
        /// <summary>
        /// Tempo de vida do objeto retornado em <see cref="GetData(PreLoadParameters)"/> na memória.
        /// </summary>
        /// <returns></returns>
        TimeSpan GetDataExpiration();

        /// <summary>
        /// Busca os dados para popular o cache do PreLoader e retorna em um Observable.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IObservable<TResponse> GetData(PreLoadParameters parameters);
    }
}
