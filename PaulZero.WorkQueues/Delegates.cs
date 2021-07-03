using PaulZero.WorkQueues.Interfaces;
using System;
using System.Threading.Tasks;

namespace PaulZero.WorkQueues
{
    public delegate Task<TResult> QueueWorkAsyncHandler<TValue, TResult>(TValue value);

    public delegate void ResultHandlerDelegate<TResult>(IWorkQueueResult<TResult> result);

    public delegate void UnhandledExceptionHandler<TValue>(TValue value, Exception exception);
}