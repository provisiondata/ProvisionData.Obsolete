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
    using System.Threading;
    using System.Threading.Tasks;
    using ProvisionData.GELF.Internal;

    public static class GelfClient
    {
        private static IGelfClient _client = NullClient.Instance;

        /// <summary>
        /// The globally-shared <see cref="IGelfClient"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IGelfClient Client {
            get => _client;
            set => _client = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Resets <see cref="Client"/> to the default and, if possible, disposes the original.
        /// </summary>
        public static void CloseAndFlush()
        {
            var client = Interlocked.Exchange(ref _client, NullClient.Instance);
            (client as IDisposable)?.Dispose();
        }

        public static async Task SendMessageAsync(Message message)
            => await _client.SendMessageAsync(message).ConfigureAwait(false);
    }
}
