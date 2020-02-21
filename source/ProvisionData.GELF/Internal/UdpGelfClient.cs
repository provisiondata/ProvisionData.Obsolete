namespace ProvisionData.GELF.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Sockets;
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

        public override async Task SendMessageAsync(Message message)
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
