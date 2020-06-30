using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Prism.Ioc;

namespace Xambon.PreLoader
{
    public static class PreLoaderExtensions
    {

        public static void RegisterPreLoaderService(this IContainerRegistry containerRegistry)
        {
            PreLoaderCore.Instance.RegisterPreLoaderService(containerRegistry);
        }


        public static bool RegisterPreLoader<TPreloader, TResponse>(this IContainerRegistry containerRegistry, string name = null) where TPreloader : IPreLoader<TResponse>
        {
            return PreLoaderCore.Instance.RegisterPreLoader<TPreloader, TResponse>(containerRegistry, name);
        }

       
    }
}
