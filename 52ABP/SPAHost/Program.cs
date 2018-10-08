using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SPAHost
{
    internal static class Program
    {
        /// <summary>
        /// 这是服务主机进程的入口点。
        /// </summary>
        private static void Main()
        {
            try
            {
                // ServiceManifest.XML 文件定义一个或多个服务类型名称。
                // 注册服务会将服务类型名称映射到 .NET 类型。
                // 在 Service Fabric 创建此服务类型的实例时，
                // 会在此主机进程中创建类的实例。

                ServiceRuntime.RegisterServiceAsync("SPAHostType",
                    context => new SPAHost(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(SPAHost).Name);

                // 防止此主机进程终止，以使服务保持运行。 
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
