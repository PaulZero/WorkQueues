using System;

namespace PaulZero.WorkQueues.Tokens
{
    internal class WorkerTokenProvider
    {
        public Guid Id { get; } = Guid.NewGuid();

        private readonly object _lock = new object();
        private readonly int _maxTokensIssued;
        private int _tokensIssued;

        public WorkerTokenProvider(int maxTokens)
        {
            if (maxTokens == 0 || maxTokens > 100)
            {
                maxTokens = 100;
            }
            else if (maxTokens < 1)
            {
                maxTokens = 1;
            }

            _maxTokensIssued = maxTokens;
        }

        public bool TryGetToken(out WorkerToken token)
        {
            lock (_lock)
            {
                if (_tokensIssued < _maxTokensIssued)
                {
                    _tokensIssued++;

                    token = new WorkerToken(this);

                    return true;
                }

                token = null;

                return false;
            }
        }

        internal bool ReleaseToken(WorkerToken token)
        {
            if (token.ProviderId != Id)
            {
                return false;
            }

            lock (_lock)
            {
                _tokensIssued--;

                return true;
            }
        }
    }
}
