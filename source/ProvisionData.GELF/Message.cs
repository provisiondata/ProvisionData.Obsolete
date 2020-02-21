namespace ProvisionData.GELF
{
    using System;
    using System.Collections.Generic;

    public class Message
    {
        private const String DefaultShortMessage = "Lazy programmer didn't set the short_message.";

        public String Version { get; } = "1.1";

        public String Host { get; set; } = Environment.MachineName;

        public String ShortMessage { get; set; } = DefaultShortMessage;

        public String? FullMessage { get; set; }

        // We do the ToUnixTime calculation here to avoid a dependency on ProvisionData.Common.
        public Double Timestamp { get; set; } = DateTime.Now.Subtract(DateTime.UnixEpoch).TotalSeconds;

        public Level Level { get; set; }

        public String? Facility { get; set; }

        public Int32? Line { get; set; }

        public String? File { get; set; }

        public IReadOnlyCollection<KeyValuePair<String, Object>> AdditionalFields { get; set; }
            = Array.Empty<KeyValuePair<String, Object>>();
    }
}