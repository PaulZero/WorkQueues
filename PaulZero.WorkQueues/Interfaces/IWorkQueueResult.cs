using System;

namespace PaulZero.WorkQueues.Interfaces
{
    public interface IWorkQueueResult<TResult>
    {
        TResult Value { get; }

        bool Success { get; }

        Exception Exception { get; }
    }
}
