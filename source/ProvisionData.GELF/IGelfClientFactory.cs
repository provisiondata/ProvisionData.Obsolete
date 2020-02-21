namespace ProvisionData.GELF
{
    using ProvisionData.GELF.Internal;

    public interface IGelfClientFactory
    {
        IGelfClient Create();
    }

    public class GelfClientFactory : IGelfClientFactory
    {
        private readonly GelfOptions _options;

        public GelfClientFactory(GelfOptions options)
        {
            _options = options ?? throw new System.ArgumentNullException(nameof(options));
        }

        public IGelfClient Create()
        {
            switch (_options.Protocol)
            {
                case Protocol.HTTP:
                case Protocol.HTTPS:
                    return new HttpGelfClient(_options);
                case Protocol.TCP:
                    return new TcpGelfClient(_options);
                default:
                    return new UdpGelfClient(_options);
            }
        }
    }
}
