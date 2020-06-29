using System.Threading.Tasks;

namespace Xambon.PreLoader
{
    public interface IPreLoader
    {
        /// <summary>
        /// Retorna os dados que devem deverão ser armazenados via PreLoader.
        /// </summary>
        /// <returns></returns>
        Task<PreLoadResult> GetDataAsync(PreLoadParameters parameters);
    }
}
