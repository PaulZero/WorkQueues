namespace PaulZero.WorkQueues.Interfaces
{
    public interface IWorkQueue<TValue, TResult> : IWorkQueueInput<TValue>, IWorkQueueOutput<TResult>
    {
        event UnhandledExceptionHandler<TValue> UnhandledException;
    }
}
