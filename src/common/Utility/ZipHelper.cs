using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;

namespace JieNor.Megi.Common.Utility
{
	public static class ZipHelper
	{
		public static string Decompress(string zippedString)
		{
			if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
			{
				return "";
			}
			byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
			return DecompressString(zippedData);
		}

		public static string DecompressString(byte[] zippedData)
		{
			if (zippedData == null || zippedData.Length == 0)
			{
				return null;
			}
			return HttpUtility.UrlDecode(Encoding.UTF8.GetString(DecompressByBytes(zippedData)));
		}

		public static T DecompressObject<T>(string zippedString)
		{
			string text = Decompress(zippedString);
			return JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});
		}

		private static byte[] ConvertIntArray2ByteArray(int[] array)
		{
			if (array == null || array.Length == 0)
			{
				return null;
			}
			byte[] result = new byte[array.Length];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = Convert.ToByte(array[i]);
			}
			return result;
		}

		private static byte[] DecompressByBytes(byte[] zippedData)
		{
			if (zippedData == null || zippedData.Length == 0)
			{
				return null;
			}
			MemoryStream ms = new MemoryStream(zippedData);
			GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
			MemoryStream outBuffer = new MemoryStream();
			byte[] block = new byte[1024];
			while (true)
			{
				int bytesRead = compressedzipStream.Read(block, 0, block.Length);
				if (bytesRead > 0)
				{
					outBuffer.Write(block, 0, bytesRead);
					continue;
				}
				break;
			}
			compressedzipStream.Close();
			return outBuffer.ToArray();
		}
	}
}
