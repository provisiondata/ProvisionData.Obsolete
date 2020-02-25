namespace ProvisionData.GELF
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using ProvisionData.GELF.Internal;

    public static class GelfClient
    {
        private static IGelfClient _client = NullClient.Instance;

        /// <summary>
        /// The globally-shared <see cref="IGelfClient"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IGelfClient Client {
            get => _client;
            set => _client = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Resets <see cref="Client"/> to the default and, if possible, disposes the original.
        /// </summary>
        public static void CloseAndFlush()
        {
            var client = Interlocked.Exchange(ref _client, NullClient.Instance);
            (client as IDisposable)?.Dispose();
        }

        public static async Task SendMessageAsync(Message message)
            => await _client.SendMessageAsync(message).ConfigureAwait(false);
    }
}
