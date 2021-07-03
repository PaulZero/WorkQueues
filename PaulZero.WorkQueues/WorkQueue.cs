using PaulZero.WorkQueues.Interfaces;
using PaulZero.WorkQueues.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaulZero.WorkQueues
{
    public class WorkQueue<TValue, TResult> : IWorkQueue<TValue, TResult>
    {
        public event UnhandledExceptionHandler<TValue> UnhandledException;
        public event ResultHandlerDelegate<TResult>  ResultReady;

        private readonly int _maxRetries;
        private readonly WorkerTokenProvider _tokenProvider;
        private readonly IList<IWorkQueueInput<TResult>> _nextQueues = new List<IWorkQueueInput<TResult>>();
        private readonly ConcurrentQueue<WorkQueueItem<TValue>> _queue = new ConcurrentQueue<WorkQueueItem<TValue>>();
        private readonly QueueWorkAsyncHandler<TValue, TResult> _workerDelegate;

        public WorkQueue(
            QueueWorkAsyncHandler<TValue, TResult> workerDelegate,
            int maxWorkers = 5,
            int maxRetries = 5
        )
        {
            _maxRetries = maxRetries;
            _tokenProvider = new WorkerTokenProvider(maxWorkers);
            _workerDelegate = workerDelegate;
        }

        public void Chain(IWorkQueueInput<TResult> nextQueue)
        {
            if (!_nextQueues.Contains(nextQueue))
            {
                _nextQueues.Add(nextQueue);
            }
        }

        public void Enqueue(TValue item, CancellationToken cancellationToken)
        {
            _queue.Enqueue(new WorkQueueItem<TValue>(item, cancellationToken));

            TryStartWorker();
        }

        private void NotifyResultReady(WorkQueueItem<TValue> queueItem, TResult result)
        {
            ResultReady?.Invoke(WorkQueueResult.Successful(result));

            foreach (var nextQueue in _nextQueues)
            {
                nextQueue.Enqueue(result, queueItem.CancellationToken);
            }
        }

        private void NotifyUnhandledException(WorkQueueItem<TValue> workQueueItem, Exception exception)
        {
            UnhandledException?.Invoke(workQueueItem.Value, exception);
            ResultReady?.Invoke(WorkQueueResult.Failed<TResult>(exception));
        }

        private void TryStartWorker(WorkerToken token = null)
        {
            if (token is null && !_tokenProvider.TryGetToken(out token))
            {
                return;
            }

            if (!_queue.TryDequeue(out var queueItem))
            {
                token?.Dispose();

                return;
            }

            var value = queueItem.Value;

            Task.Run(async () => await _workerDelegate(value))
                .ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        NotifyResultReady(queueItem, t.Result);
                    }
                    else
                    {
                        var retryQueueItem = queueItem.WithIncrementedRetryCount();

                        if (retryQueueItem.RetryCount <= _maxRetries)
                        {
                            _queue.Enqueue(retryQueueItem);
                        }
                        else
                        {
                            NotifyUnhandledException(
                                queueItem,
                                new Exception("Retried task a bunch of times and still failed.", t.Exception)
                            );
                        }
                    }

                    TryStartWorker(token);
                });
        }
    }
}
