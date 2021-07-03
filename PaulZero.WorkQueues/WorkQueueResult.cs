using PaulZero.WorkQueues.Interfaces;
using System;

namespace PaulZero.WorkQueues
{
    internal class WorkQueueResult<TResult> : IWorkQueueResult<TResult>
    {
        public TResult Value { get; }

        public bool Success { get; }

        public Exception Exception { get; }

        public WorkQueueResult(TResult value, bool success, Exception exception)
        {
            Value = value;
            Success = success;
            Exception = exception;
        }
    }

    internal static class WorkQueueResult
    {
        public static IWorkQueueResult<TResult> Successful<TResult>(TResult value)
            => new WorkQueueResult<TResult>(value, true, null);

        public static IWorkQueueResult<TResult> Failed<TResult>(Exception exception)
            => new WorkQueueResult<TResult>(default, false, exception);
    }
}
