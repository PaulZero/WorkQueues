using System.Threading;

namespace PaulZero.WorkQueues
{
    internal class WorkQueueItem<TValue>
    {
        public CancellationToken CancellationToken { get; }

        public TValue Value { get; }

        public int RetryCount { get; }

        public WorkQueueItem(TValue value, CancellationToken cancellationToken, int retryCount = 0)
        {
            CancellationToken = cancellationToken;
            Value = value;
            RetryCount = retryCount;
        }

        public WorkQueueItem<TValue> WithIncrementedRetryCount()
            => new WorkQueueItem<TValue>(Value, CancellationToken, RetryCount + 1);
    }
}
