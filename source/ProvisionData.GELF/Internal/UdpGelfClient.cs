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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    internal class UdpGelfClient : GelfClientBase
    {
        private const Int32 MaxChunks = 128;
        private const Int32 MaxChunkSize = 8192;
        private const Int32 MessageHeaderSize = 12;
        private const Int32 MessageIdSize = 8;
        private const Int32 MaxMessageBodySize = MaxChunkSize - MessageHeaderSize;

        private readonly UdpClient _udpClient;
        private readonly Random _random;

        public UdpGelfClient(GelfOptions options)
            : base(options)
        {
            _udpClient = new UdpClient();
            _random = new Random();
        }

        public override async Task SendMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            var payload = message.GetBytes();

            if (Options.CompressUdp && payload.Length > Options.UdpCompressionThreshold)
            {
                payload = await CompressMessageAsync(payload).ConfigureAwait(false);
            }

            foreach (var messageChunk in ChunkMessage(payload))
            {
                await _udpClient.SendAsync(messageChunk, messageChunk.Length, IPEndPoint).ConfigureAwait(false);
            }
        }

        private static async Task<Byte[]> CompressMessageAsync(Byte[] messageBytes)
        {
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                await gzipStream.WriteAsync(messageBytes, 0, messageBytes.Length).ConfigureAwait(false);
            }
            return outputStream.ToArray();
        }

        private IEnumerable<Byte[]> ChunkMessage(Byte[] messageBytes)
        {
            if (messageBytes.Length < MaxChunkSize)
            {
                yield return messageBytes;
                yield break;
            }

            var sequenceCount = (Int32)Math.Ceiling(messageBytes.Length / (Double)MaxMessageBodySize);
            if (sequenceCount > MaxChunks)
            {
                Debug.Fail($"GELF message contains {sequenceCount} chunks, exceeding the maximum of {MaxChunks}.");
                yield break;
            }

            var messageId = GetMessageId();
            for (var sequenceNumber = 0; sequenceNumber < sequenceCount; sequenceNumber++)
            {
                var messageHeader = GetMessageHeader(sequenceNumber, sequenceCount, messageId);
                var chunkStartIndex = sequenceNumber * MaxMessageBodySize;
                var messageBodySize = Math.Min(messageBytes.Length - chunkStartIndex, MaxMessageBodySize);
                var chunk = new Byte[messageBodySize + MessageHeaderSize];

                Array.Copy(messageHeader, chunk, MessageHeaderSize);
                Array.ConstrainedCopy(messageBytes, chunkStartIndex, chunk, MessageHeaderSize, messageBodySize);

                yield return chunk;
            }
        }

        [SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "It's sufficient for this use case.")]
        private Byte[] GetMessageId()
        {
            var messageId = new Byte[8];
            _random.NextBytes(messageId);
            return messageId;
        }

        private static Byte[] GetMessageHeader(Int32 sequenceNumber, Int32 sequenceCount, Byte[] messageId)
        {
            var header = new Byte[MessageHeaderSize];
            header[0] = 0x1e;
            header[1] = 0x0f;

            Array.ConstrainedCopy(messageId, 0, header, 2, MessageIdSize);

            header[10] = Convert.ToByte(sequenceNumber);
            header[11] = Convert.ToByte(sequenceCount);

            return header;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _udpClient.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
