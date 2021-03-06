using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Containers.Extensions;
using H.Utilities;

namespace H.Containers
{
    public static class ChildProgram
    {
        private static CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        private static PipeProxyServer ProxyServer { get; } = new PipeProxyServer();

        public static async Task Main(string[] arguments, bool isSecondProcess)
        {
            try
            {
                var parent = isSecondProcess
                    ? Process.GetCurrentProcess().GetParent()
                    : Process.GetCurrentProcess();
                if (arguments.Length < 1)
                {
                    return;
                }

                var name = arguments.ElementAt(0);

                ProxyServer.MessageReceived += async (sender, message) =>
                {
                    await OnMessageReceivedAsync(message);
                };
                ProxyServer.ExceptionOccurred += (sender, exception) =>
                {
                    Console.Error.WriteLine($"Server Exception: {exception}");
                };

                await ProxyServer.InitializeAsync(name, CancellationTokenSource.Token);

                while (!CancellationTokenSource.IsCancellationRequested && 
                       (parent == null || !parent.HasExited))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1), CancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                ProxyServer.Dispose();
                CancellationTokenSource.Dispose();
            }
        }

        private static async Task OnMessageReceivedAsync(string message)
        {
            try
            {
                switch (message)
                {
                    case "stop":
                        CancellationTokenSource.Cancel();
                        break;
                }
            }
            catch (Exception exception)
            {
                await ProxyServer.SendExceptionAsync(exception);
            }
        }
    }
}
