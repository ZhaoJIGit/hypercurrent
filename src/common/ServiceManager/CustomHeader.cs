using JieNor.Megi.Common.Context;
using System.IO;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;

namespace JieNor.Megi.Common.ServiceManager
{
	public class CustomHeader : MessageHeader
	{
		private const string CUSTOM_HEADER_NAMESPACE = "http://tempuri.org";

		private ServiceHeader _customData;

		public ServiceHeader CustomData => _customData;

		public override string Name => ContextHelper.MAccessTokenCookie;

		public override string Namespace => "http://tempuri.org";

		public CustomHeader()
		{
		}

		public CustomHeader(ServiceHeader customData)
		{
			_customData = customData;
		}

		protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ServiceHeader));
			StringWriter textWriter = new StringWriter();
			serializer.Serialize(textWriter, _customData);
			textWriter.Close();
			string text = textWriter.ToString();
			writer.WriteElementString(ContextHelper.MAccessTokenCookie, "Key", text.Trim());
		}

		public static ServiceHeader ReadHeader(Message request)
		{
			int headerPosition = request.Headers.FindHeader(ContextHelper.MAccessTokenCookie, "http://tempuri.org");
			if (headerPosition == -1)
			{
				return null;
			}
			MessageHeaderInfo headerInfo = request.Headers[headerPosition];
			XmlNode[] content = request.Headers.GetHeader<XmlNode[]>(headerPosition);
			string text = content[0].InnerText;
			XmlSerializer deserializer = new XmlSerializer(typeof(ServiceHeader));
			TextReader textReader = new StringReader(text);
			ServiceHeader customData = (ServiceHeader)deserializer.Deserialize(textReader);
			textReader.Close();
			return customData;
		}
	}
}
