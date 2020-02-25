﻿namespace ProvisionData.GELF.Internal
{
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class NullClient : IGelfClient
    {
        public static readonly IGelfClient Instance = new NullClient();

        private NullClient()
        {
        }

        public void Dispose() { }

        public Task SendMessageAsync(Message message, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
