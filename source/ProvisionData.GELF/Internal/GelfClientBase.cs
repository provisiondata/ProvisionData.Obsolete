namespace ProvisionData.GELF.Internal
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    internal abstract class GelfClientBase : IGelfClient
    {
        protected readonly GelfOptions Options;
        protected readonly IPEndPoint IPEndPoint;

        protected GelfClientBase(GelfOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            IPEndPoint = ResolveIpAddress(Options);
        }

        protected static IPEndPoint ResolveIpAddress(GelfOptions options)
        {
            var host = GetHost(options.Host);
            var ipAddreses = Dns.GetHostAddresses(host);
            var ipAddress = Array.Find(ipAddreses, c => c.AddressFamily == AddressFamily.InterNetwork);
            return new IPEndPoint(ipAddress ?? throw new InvalidOperationException($"Unable to resolve {host}:{options.Port}."), options.Port);
        }

        private static String GetHost(String host)
        {
            return host.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                ? new UriBuilder(host).Host
                : host;
        }

        public abstract Task SendMessageAsync(Message message);

        protected virtual void Dispose(Boolean disposing) { }

        public void Dispose() => Dispose(true);
    }
}
