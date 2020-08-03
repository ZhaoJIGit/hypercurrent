using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace JieNor.Megi.GZipEncoder
{
	public sealed class GZipMessageEncodingBindingElement : MessageEncodingBindingElement, IPolicyExportExtension
	{
		private MessageEncodingBindingElement innerBindingElement;

		public MessageEncodingBindingElement InnerMessageEncodingBindingElement
		{
			get
			{
				return innerBindingElement;
			}
			set
			{
				innerBindingElement = value;
			}
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return innerBindingElement.MessageVersion;
			}
			set
			{
				innerBindingElement.MessageVersion = value;
			}
		}

		public GZipMessageEncodingBindingElement()
			: this(new TextMessageEncodingBindingElement())
		{
		}

		public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement)
		{
			innerBindingElement = messageEncoderBindingElement;
		}

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new GZipMessageEncoderFactory(innerBindingElement.CreateMessageEncoderFactory());
		}

		public override BindingElement Clone()
		{
			return new GZipMessageEncodingBindingElement(innerBindingElement);
		}

		public override T GetProperty<T>(BindingContext context)
		{
			if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
			{
				return innerBindingElement.GetProperty<T>(context);
			}
			return base.GetProperty<T>(context);
		}

		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			((Collection<object>)context.BindingParameters).Add((object)this);
			return context.BuildInnerChannelFactory<TChannel>();
		}

		public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			((Collection<object>)context.BindingParameters).Add((object)this);
			return context.BuildInnerChannelListener<TChannel>();
		}

		public override bool CanBuildChannelListener<TChannel>(BindingContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			((Collection<object>)context.BindingParameters).Add((object)this);
			return context.CanBuildInnerChannelListener<TChannel>();
		}

		void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
		{
			if (policyContext == null)
			{
				throw new ArgumentNullException("policyContext");
			}
			XmlDocument xmlDocument = new XmlDocument();
			policyContext.GetBindingAssertions().Add(xmlDocument.CreateElement("gzip", "GZipEncoding", "http://schemas.microsoft.com/ws/06/2004/mspolicy/netgzip1"));
		}
	}
}
