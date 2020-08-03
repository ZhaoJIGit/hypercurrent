using System;
using System.IO;
using System.IO.Compression;
using System.ServiceModel.Channels;

namespace JieNor.Megi.GZipEncoder
{
	internal class GZipMessageEncoderFactory : MessageEncoderFactory
	{
		private class GZipMessageEncoder : MessageEncoder
		{
			private static string GZipContentType = "application/x-gzip";

			private MessageEncoder innerEncoder;

			public override string ContentType => GZipContentType;

			public override string MediaType => GZipContentType;

			public override MessageVersion MessageVersion => innerEncoder.MessageVersion;

			internal GZipMessageEncoder(MessageEncoder messageEncoder)
			{
				if (messageEncoder == null)
				{
					throw new ArgumentNullException("messageEncoder", "A valid message encoder must be passed to the GZipEncoder");
				}
				innerEncoder = messageEncoder;
			}

			private static ArraySegment<byte> CompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager, int messageOffset)
			{
				MemoryStream memoryStream = new MemoryStream();
				memoryStream.Write(buffer.Array, 0, messageOffset);
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gZipStream.Write(buffer.Array, messageOffset, buffer.Count);
				}
				byte[] array = memoryStream.ToArray();
				byte[] array2 = bufferManager.TakeBuffer(array.Length);
				Array.Copy(array, 0, array2, 0, array.Length);
				bufferManager.ReturnBuffer(buffer.Array);
				return new ArraySegment<byte>(array2, messageOffset, array2.Length - messageOffset);
			}

			private static ArraySegment<byte> DecompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager)
			{
				MemoryStream stream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count - buffer.Offset);
				MemoryStream memoryStream = new MemoryStream();
				int num = 0;
				int num2 = 1024;
				byte[] buffer2 = bufferManager.TakeBuffer(num2);
				using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					while (true)
					{
						int num3 = gZipStream.Read(buffer2, 0, num2);
						if (num3 != 0)
						{
							memoryStream.Write(buffer2, 0, num3);
							num += num3;
							continue;
						}
						break;
					}
				}
				bufferManager.ReturnBuffer(buffer2);
				byte[] array = memoryStream.ToArray();
				byte[] array2 = bufferManager.TakeBuffer(array.Length + buffer.Offset);
				Array.Copy(buffer.Array, 0, array2, 0, buffer.Offset);
				Array.Copy(array, 0, array2, buffer.Offset, array.Length);
				ArraySegment<byte> result = new ArraySegment<byte>(array2, buffer.Offset, array.Length);
				bufferManager.ReturnBuffer(buffer.Array);
				return result;
			}

			public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
			{
				ArraySegment<byte> buffer2 = DecompressBuffer(buffer, bufferManager);
				Message message = innerEncoder.ReadMessage(buffer2, bufferManager);
				message.Properties.Encoder = this;
				return message;
			}

			public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
			{
				ArraySegment<byte> buffer = innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
				return CompressBuffer(buffer, bufferManager, messageOffset);
			}

			public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
			{
				GZipStream stream2 = new GZipStream(stream, CompressionMode.Decompress, true);
				return innerEncoder.ReadMessage(stream2, maxSizeOfHeaders);
			}

			public override void WriteMessage(Message message, Stream stream)
			{
				using (GZipStream stream2 = new GZipStream(stream, CompressionMode.Compress, true))
				{
					innerEncoder.WriteMessage(message, stream2);
				}
				stream.Flush();
			}
		}

		private MessageEncoder encoder;

		public override MessageEncoder Encoder => encoder;

		public override MessageVersion MessageVersion => encoder.MessageVersion;

		public GZipMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory)
		{
			if (messageEncoderFactory == null)
			{
				throw new ArgumentNullException("messageEncoderFactory", "A valid message encoder factory must be passed to the GZipEncoder");
			}
			encoder = new GZipMessageEncoder(messageEncoderFactory.Encoder);
		}
	}
}
