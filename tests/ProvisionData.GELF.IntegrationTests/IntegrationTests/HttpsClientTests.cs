namespace ProvisionData.GELF.IntegrationTests
{
    using ProvisionData.GELF.Internal;

    public class HttpsClientTests : GelfClientTests
    {
        protected override IGelfClient GetClient() => new HttpGelfClient(new GelfOptions()
        {
            Host = "https://graylog.pdsint.net/gelf"
        });
    }
}
