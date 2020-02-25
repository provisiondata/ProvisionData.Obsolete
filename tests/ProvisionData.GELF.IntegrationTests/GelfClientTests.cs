namespace ProvisionData.GELF
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public abstract class GelfClientTests
    {
        protected abstract IGelfClient GetClient();

        //[Fact]
        //public async Task Send_raw_message_to_Graylog()
        //{
        //    var message = $"{{ \"version\": \"1.1\", \"host\": \"{Environment.MachineName}\", \"short_message\": \"Provision Data Systems Inc. GelfTest\", \"level\": 5, \"facility\": \"{GetType().Name}\" }}";
        //    var payload = Encoding.UTF8.GetBytes(message);
        //    using var client = new UdpClient();
        //    await client.SendAsync(payload, payload.Length, "graylog.pdsint.net", 12201).ConfigureAwait(false);
        //}

        [Fact]
        public async Task Send_message_to_Graylog()
        {
            using var client = GetClient();
            var message = new Message()
            {
                ShortMessage = $"GelfTest {DateTime.Now}",
                Level = Level.Debug
            };

            await client.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
