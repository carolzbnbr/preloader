using System;
using System.Threading.Tasks;

namespace Xambon.PreLoader
{

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
