using System.IO;
using System.Xml.Serialization;

namespace JieNor.Megi.Common.Utility
{
	public static class XMLUtility
	{
		public static T XmlToObject<T>(string xmlString)
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			StringReader sr = new StringReader(xmlString);
			return (T)xs.Deserialize(sr);
		}

		public static string ObjectToXml<T>(T obj)
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			StringWriter sw = new StringWriter();
			xs.Serialize(sw, obj);
			return sw.ToString();
		}
	}
}
