using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xambon.PreLoader.Extensions
{
    public static class PreLoaderAkavacheExtensions
    {
        private static EventLoopScheduler Scheduler;
        static PreLoaderAkavacheExtensions()
        {
            Scheduler = new EventLoopScheduler(ts => new Thread(ts) { IsBackground = true });
        }
    


        /// <summary>
        /// Tenta obter o dado do cache do PreLoader, caso nao seja encontrado, força a execucao do preloader para obter os dados, atualiza o cache e retorna os novos dados obtidos.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="preLoaderService"></param>
        /// <param name="preLoaderName"></param>
        /// <returns></returns>
        public static  IObservable<T> GetOrInvokePreLoader<T>(this IPreLoaderService preLoaderService, string preLoaderName, PreLoadParameters parameters) where T : class
        {

            //        urls.ToObservable()
            //.Select(url => Observable.FromAsync(async () => {
            //    var bytes = await this.DownloadImage(url);
            //    var image = await this.ParseImage(bytes);
            //    return image;
            //}))
            //.Merge(6 /*at a time*/);

            var fetch = Observable.Start(() => preLoaderService.GetPreLoadedData<T>(preLoaderName))
            .SelectMany(response => Observable.FromAsync(async () =>
            {
                if (response != default(T))
                {
                    return response;
                }

                //Forca a execucao do pre-loader...
                var observable = preLoaderService.InvokePreLoader(preLoaderName, parameters).ToObservable(Scheduler);//.ObserveOn(Scheduler);
                var result = await observable.LastOrDefaultAsync();

                //tenta obter o resultado do preloader...
                response = preLoaderService.GetPreLoadedData<T>(preLoaderName);

               
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
        public static async Task<T> GetOrInvokePreLoader<T>(this IPreLoaderService preLoaderService, PreLoadParameters parameters, string preLoaderName) where T : class
        {
            var response = preLoaderService.GetPreLoadedData<T>(preLoaderName);
            if (response != default(T))
            {
                return response;
            }
            else //null
            {

                //Forca a execucao do pre-loader...
                await preLoaderService.InvokePreLoader(preLoaderName, parameters);

                //tenta obter o resultado do preloader...
                response = preLoaderService.GetPreLoadedData<T>(preLoaderName);

                return response;
            }
        }
    }
}
