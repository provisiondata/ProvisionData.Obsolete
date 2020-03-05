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
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TcpGelfClient : GelfClientBase
    {
        private readonly TcpClient _client;
        private NetworkStream? _stream;

        public TcpGelfClient(GelfOptions options)
            : base(options)
        {
            _client = new TcpClient();
        }

        public override async Task SendMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (_stream is null)
                _stream = await ConnectAsync().ConfigureAwait(false);

            var json = message.GetJson();
            Debug.WriteLine(json);
            var payload = message.GetBytes();

            Array.Resize(ref payload, payload.Length + 1);
            payload[^1] = 0x00;

            await _stream.WriteAsync(payload, 0, payload.Length).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }

        private async Task<NetworkStream> ConnectAsync()
        {
            await _client.ConnectAsync(IPEndPoint.Address, IPEndPoint.Port).ConfigureAwait(false);
            return _client.GetStream();
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _client.Close();
                _client.Dispose();
                _stream?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
