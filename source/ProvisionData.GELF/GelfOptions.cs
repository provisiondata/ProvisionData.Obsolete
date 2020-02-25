namespace ProvisionData.GELF
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Graylog Extended Log Format Options
    /// </summary>
    public class GelfOptions
    {
        /// <summary>
        /// The 
        /// </summary>
        public Protocol Protocol { get; set; } = Protocol.UDP;

        /// <summary>
        /// GELF server host.
        /// </summary>
        public String Host { get; set; } = "localhost";

        /// <summary>
        /// GELF server port.
        /// </summary>
        public Int32 Port { get; set; } = 12201;

        /// <summary>
        /// Enable GZip message compression for UDP logging.
        /// </summary>
        public Boolean CompressUdp { get; set; } = true;

        /// <summary>
        /// The UDP message size in bytes under which messages will not be compressed.
        /// </summary>
        public Int32 UdpCompressionThreshold { get; set; } = 512;

        /// <summary>
        /// Headers used when sending logs via HTTP(S).
        /// </summary>
        public Dictionary<String, String> HttpHeaders { get; set; } = new Dictionary<String, String>();

        /// <summary>
        /// Timeout used when sending logs via HTTP(S).
        /// </summary>
        public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}
