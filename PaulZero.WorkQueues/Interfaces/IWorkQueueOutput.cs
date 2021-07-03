namespace PaulZero.WorkQueues.Interfaces
{
    public interface IWorkQueueOutput<TResult>
    {
        event ResultHandlerDelegate<TResult> ResultReady;
    }
}
