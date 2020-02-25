namespace ProvisionData.GELF.IntegrationTests
{
    using ProvisionData.GELF.Internal;

    public class UdpClientTests : GelfClientTests
    {
        // tcpdump -A -i em1 udp port 12201
        protected override IGelfClient GetClient() => new UdpGelfClient(new GelfOptions()
        {
            Host = "graylog.pdsint.net",
            //UdpCompressionThreshold = 100
        });
    }
}
