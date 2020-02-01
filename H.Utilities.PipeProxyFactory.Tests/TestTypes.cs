﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Utilities.Tests
{
    public delegate void TextDelegate(string text);

    public interface ISimpleEventClass
    {
        event EventHandler<int> Event1;
        event TextDelegate? Event3;

        void RaiseEvent1();
        void RaiseEvent3();
        int Method1(int input);
        string Method2(string input);
    }

    public class SimpleEventClass : ISimpleEventClass
    {
        public event EventHandler<int>? Event1;
        public event TextDelegate? Event3;

        public void RaiseEvent1()
        {
            Event1?.Invoke(this, 777);
        }

        public void RaiseEvent3()
        {
            Event3?.Invoke("555");
        }

        public int Method1(int input)
        {
            return 321 + input;
        }

        public string Method2(string input)
        {
            return $"Hello, input = {input}";
        }
    }

    #region Async methods

    public interface IAsyncMethodsClass
    {
        Task NoValueTaskAsync(CancellationToken cancellationToken = default);
        Task<int> ValueTypeResultTaskWithResultEquals1Async(CancellationToken cancellationToken = default);
        Task InvalidOperationExceptionAfterSecondTaskAsync(CancellationToken cancellationToken = default);
    }

    public class AsyncMethodsClass : IAsyncMethodsClass
    {
        public async Task NoValueTaskAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        public async Task<int> ValueTypeResultTaskWithResultEquals1Async(CancellationToken cancellationToken = default)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            return 1;
        }

        public async Task InvalidOperationExceptionAfterSecondTaskAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            throw new InvalidOperationException("Custom exception message");
        }
    }

    #endregion
}
