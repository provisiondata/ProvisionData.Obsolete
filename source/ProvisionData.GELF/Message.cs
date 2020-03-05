/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

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