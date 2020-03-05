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

namespace ProvisionData.GELF.Internal
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    internal abstract class GelfClientBase : IGelfClient
    {
        protected readonly GelfOptions Options;
        protected readonly IPEndPoint IPEndPoint;

        protected GelfClientBase(GelfOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            IPEndPoint = ResolveIpAddress(Options);
        }

        protected static IPEndPoint ResolveIpAddress(GelfOptions options)
        {
            var host = GetHost(options.Host);
            var ipAddreses = Dns.GetHostAddresses(host);
            var ipAddress = Array.Find(ipAddreses, c => c.AddressFamily == AddressFamily.InterNetwork);
            return new IPEndPoint(ipAddress ?? throw new InvalidOperationException($"Unable to resolve {host}:{options.Port}."), options.Port);
        }

        private static String GetHost(String host)
        {
            return host.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                ? new UriBuilder(host).Host
                : host;
        }

        public abstract Task SendMessageAsync(Message message, CancellationToken cancellationToken = default);

        protected virtual void Dispose(Boolean disposing) { }

        public void Dispose() => Dispose(true);
    }
}
