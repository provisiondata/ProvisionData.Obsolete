namespace ProvisionData.GELF
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Graylog Extended Log Format Options
    /// </summary>
    public class GelfOptions
    {
        public GelfOptions()
        {
            Protocol = Protocol.UDP;
            Host = "localhost";
            Port = 12201;
            CompressUdp = true;
            UdpCompressionThreshold = 512;
            HttpHeaders = Array.Empty<HttpHeader>();
            HttpTimeout = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// The transport <see cref="Protocol"/> to use.
        /// </summary>
        public Protocol Protocol { get; set; }

        /// <summary>
        /// GELF server host.
        /// </summary>
        public String Host { get; set; }

        /// <summary>
        /// GELF server port.
        /// </summary>
        public Int32 Port { get; set; }

        /// <summary>
        /// Enable GZip message compression for UDP logging.
        /// </summary>
        public Boolean CompressUdp { get; set; }

        /// <summary>
        /// The UDP message size in bytes under which messages will not be compressed.
        /// </summary>
        public Int32 UdpCompressionThreshold { get; set; }

        /// <summary>
        /// Headers used when sending logs via HTTP(S).
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is the best way for Options classes.")]
        public HttpHeader[] HttpHeaders { get; set; }

        /// <summary>
        /// Timeout used when sending logs via HTTP(S).
        /// </summary>
        public TimeSpan HttpTimeout { get; set; }

        public class HttpHeader
        {
            public HttpHeader()
            {
                Name = String.Empty;
                Value = String.Empty;
            }

            public String Name { get; set; }
            public String Value { get; set; }
        }
    }
}
