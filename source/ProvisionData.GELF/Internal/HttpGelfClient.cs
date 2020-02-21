namespace ProvisionData.GELF.Internal
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    internal class HttpGelfClient : GelfClientBase
    {
        private readonly HttpClient _httpClient;

        public HttpGelfClient(GelfOptions options)
            : base(options)
        {
            var uriBuilder = new UriBuilder(options.Host);

            _httpClient = new HttpClient
            {
                BaseAddress = uriBuilder.Uri,
                Timeout = options.HttpTimeout
            };

            if (options.HttpHeaders != null)
            {
                foreach (var header in options.HttpHeaders)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        public override async Task SendMessageAsync(Message message)
        {
            var content = new StringContent(message.GetJson(), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("gelf", content).ConfigureAwait(false);
            result.EnsureSuccessStatusCode();
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}