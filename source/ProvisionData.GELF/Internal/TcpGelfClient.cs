namespace ProvisionData.GELF.Internal
{
    using System;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    internal class TcpGelfClient : GelfClientBase
    {
        private readonly TcpClient _client;
        private NetworkStream? _stream;

        public TcpGelfClient(GelfOptions options)
            : base(options)
        {
            _client = new TcpClient();
        }

        public override async Task SendMessageAsync(Message message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (_stream is null)
                _stream = await ConnectAsync().ConfigureAwait(false);

            var json = message.GetJson();
            Debug.WriteLine(json);
            var payload = message.GetBytes();

            Array.Resize(ref payload, payload.Length + 1);
            payload[^1] = 0x00;

            await _stream.WriteAsync(payload, 0, payload.Length).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }

        private async Task<NetworkStream> ConnectAsync()
        {
            await _client.ConnectAsync(IPEndPoint.Address, IPEndPoint.Port).ConfigureAwait(false);
            return _client.GetStream();
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _client.Close();
                _client.Dispose();
                _stream?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
