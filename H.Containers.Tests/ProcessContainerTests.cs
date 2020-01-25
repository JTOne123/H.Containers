﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Containers.Tests
{
    [TestClass]
    public class ProcessContainerTests
    {
        [TestMethod]
        public async Task StartTest()
        {
            var receivedException = (Exception?) null;
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            await using var container = new ProcessContainer(nameof(ProcessContainerTests))
            {
                ForceUpdateApplication = true,
            };
            container.ExceptionOccurred += (sender, exception) =>
            {
                receivedException = exception;

                // ReSharper disable once AccessToDisposedClosure
                cancellationTokenSource.Cancel();
            };

            await container.InitializeAsync(cancellationTokenSource.Token);
            await container.StartAsync(cancellationTokenSource.Token);
            //await container.LoadAssemblyAsync("test", cancellationTokenSource.Token);

            var test = await container.CreateObjectAsync<ITest>("Test", cancellationTokenSource.Token);

            var result = test.Test1("hello");
            Console.WriteLine($"Result: {result}");
            test.Test2();

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                if (receivedException != null)
                {
                    Assert.Fail(receivedException.ToString());
                }
            }
        }

        public interface ITest
        {
            int Test1(string test);
            void Test2();
        }
    }
}
