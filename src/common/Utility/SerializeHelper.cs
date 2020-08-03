using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Common.Utility
{
	public class SerializeHelper
	{
		public static string XmlSerialize<T>(T obj)
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(T));
			MemoryStream stream = new MemoryStream();
			serializer.WriteObject(stream, obj);
			stream.Position = 0L;
			StreamReader sr = new StreamReader(stream);
			string resultStr = sr.ReadToEnd();
			sr.Close();
			stream.Close();
			return resultStr;
		}

		public static T XmlDeserialize<T>(string xml)
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(T));
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml.ToCharArray()));
			T obj = (T)serializer.ReadObject(ms);
			ms.Close();
			return obj;
		}

		public static string JsonSerialize<T>(T obj)
		{
			JavaScriptSerializer jss = new JavaScriptSerializer();
			return jss.Serialize(obj);
		}

		public static T JsonDeserialize<T>(string json)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json.ToCharArray()));
			T obj = (T)serializer.ReadObject(ms);
			ms.Close();
			return obj;
		}
	}
}
