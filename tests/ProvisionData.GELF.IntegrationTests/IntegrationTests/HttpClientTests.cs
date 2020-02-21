namespace ProvisionData.GELF.IntegrationTests
{
    using ProvisionData.GELF.Internal;

    public class HttpClientTests : GelfClientTests
    {
        protected override IGelfClient GetClient() => new HttpGelfClient(new GelfOptions()
        {
            Host = "http://graylog.pdsint.net/gelf"
        });
    }
}
