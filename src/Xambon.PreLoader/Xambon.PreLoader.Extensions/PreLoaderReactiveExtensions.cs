using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Xambon.PreLoader.Extensions
{
    public static class PreLoaderReactiveExtensions
    {
        private static EventLoopScheduler Scheduler;
        static PreLoaderReactiveExtensions()
        {
            Scheduler = new EventLoopScheduler(ts => new Thread(ts) { IsBackground = true });
        }


        /// <summary>
        /// Tenta retornar o dado do cache do PreLoader e caso nao seja encontrado força a execucao do preloader para obter os dados, atualiza o cache do PreLoader e retorna os novos dados obtidos.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="preLoaderService"></param>
        /// <param name="preLoaderName"></param>
        /// <returns></returns>
        public static IObservable<T> GetOrInvokePreLoader<T>(this IPreLoaderService preLoaderService, string preLoaderName, PreLoadParameters parameters = null) where T : class
        {

            var fetch = Observable.Start<T>(() => PreLoaderCore.Instance.GetCachedPreLoadedData<T>(preLoaderName, parameters), Scheduler)
            .SelectMany(response => Observable.FromAsync(async () =>
            {
                if (response != null)
                {
                    return response; //retorna o dado do cache do preloader
                }
                Debug.WriteLine($"====PreLoaderReactiveExtensions.GetOrInvokePreLoaderObservable() - Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                //Forca a execucao do pre-loader...
                var observable = PreLoaderCore.Instance.InvokePreLoaderWithObservable<T>(preLoaderName, parameters);

                await observable.ForEachAsync(x => Debug.WriteLine(x));

                //tenta obter o resultado do preloader...
                response = PreLoaderCore.Instance.GetCachedPreLoadedData<T>(preLoaderName, parameters);

                return response;

            }));
            return fetch;
        }

        /// <summary>
        /// Tenta obter o dado em memoria do preloader, caso nao seja encontrado, executa o força a execucao do preloader para obter os dados, tenta obter os dados, e retorna.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="preLoaderService"></param>
        /// <param name="preLoaderName"></param>
        /// <param name="fetchFunc"></param>
        /// <returns></returns>
        public static async Task<T> GetOrInvokePreLoaderAsync<T>(this IPreLoaderService preLoaderService, string preloaderName, PreLoadParameters parameters = null) where T : class
        {
            var response = PreLoaderCore.Instance.GetCachedPreLoadedData<T>(preloaderName, parameters);
            if (response != default(T))
            {
                return response;
            }
            else //null
            {

                //Dispara a execucao do Preloader da implementacao do usuario
                var observable = PreLoaderCore.Instance.InvokePreLoaderWithObservable<T>(preloaderName, parameters);
                observable.Subscribe(f => Debug.WriteLine($"Invoked {nameof(PreLoaderReactiveExtensions)}.{nameof(PreLoaderReactiveExtensions.GetOrInvokePreLoaderAsync)}"));

                await observable.ForEachAsync(x => Debug.WriteLine(x));

                //tenta obter o resultado resultante da execucao do preloader...
                response = PreLoaderCore.Instance.GetCachedPreLoadedData<T>(preloaderName, parameters);

                return response;
            }
        }




    }
}
