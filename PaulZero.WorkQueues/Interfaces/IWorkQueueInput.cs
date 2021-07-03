using System.Threading;

namespace PaulZero.WorkQueues.Interfaces
{
    public interface IWorkQueueInput<TValue>
    {
        void Enqueue(TValue value, CancellationToken cancellationToken);
    }
}
