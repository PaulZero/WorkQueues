using System;

namespace PaulZero.WorkQueues.Tokens
{
    internal class WorkerToken : IDisposable
    {
        public Guid ProviderId => _provider.Id;

        private bool _hasDisposed;
        private readonly WorkerTokenProvider _provider;

        public WorkerToken(WorkerTokenProvider provider)
        {
            _provider = provider;
        }

        public void Dispose()
        {
            if (_hasDisposed)
            {
                return;
            }

            _provider.ReleaseToken(this);
            _hasDisposed = true;
        }
    }
}
