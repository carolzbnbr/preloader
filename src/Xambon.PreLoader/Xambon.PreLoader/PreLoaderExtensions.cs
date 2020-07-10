using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Prism.Ioc;

namespace Xambon.PreLoader
{
    public static class PreLoaderExtensions
    {


        /// <summary>
        ///Register the preloader service which will allow us to consume our preloader
        ///implementation through <see cref="IPreLoaderService"/>  from the ViewModels classes.
        /// </summary>
        /// <param name="containerRegistry"></param>
        public static void RegisterPreLoaderService(this IContainerRegistry containerRegistry)
        {
            PreLoaderServiceCore.Instance.RegisterPreLoaderService(containerRegistry);
        }

        /// <summary>
        /// register your preloader implementation, so that it can be consumed inside the view models through "IPreloaderService" interfac, via PRISM Dependency injection.
        /// </summary>
        /// <typeparam name="TPreloader">Corresponds to your preloader implementation</typeparam>
        /// <typeparam name="TResponse">Corresponds to the returning type of the your data that your preloader implements</typeparam>
        /// <param name="containerRegistry"></param>
        /// <param name="preLoaderName">An alias of your preloader implementation. This name must be unique among other preloaders you may have</param>
        /// <returns></returns>
        public static bool RegisterPreLoader<TPreloader, TResponse>(this IContainerRegistry containerRegistry, string preLoaderName = null) where TPreloader : IPreLoader<TResponse>
        {
            return PreLoaderServiceCore.Instance.RegisterPreLoader<TPreloader, TResponse>(containerRegistry, preLoaderName);
        }

       
    }
}
