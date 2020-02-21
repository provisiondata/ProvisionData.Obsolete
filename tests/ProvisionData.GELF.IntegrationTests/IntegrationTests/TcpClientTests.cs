namespace ProvisionData.GELF.IntegrationTests
{
    using ProvisionData.GELF.Internal;

    // tcpdump -A -i em1 tcpp port 12201
    public class TcpClientTests : GelfClientTests
    {
        protected override IGelfClient GetClient() => new TcpGelfClient(new GelfOptions()
        {
            Host = "graylog.pdsint.net"
        });
    }
}
