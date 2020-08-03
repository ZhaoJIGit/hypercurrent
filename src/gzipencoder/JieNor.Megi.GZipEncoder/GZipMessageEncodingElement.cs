using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace JieNor.Megi.GZipEncoder
{
	public class GZipMessageEncodingElement : BindingElementExtensionElement
	{
		public override Type BindingElementType => typeof(GZipMessageEncodingBindingElement);

		[ConfigurationProperty("innerMessageEncoding", DefaultValue = "textMessageEncoding")]
		public string InnerMessageEncoding
		{
			get
			{
				return (string)base["innerMessageEncoding"];
			}
			set
			{
				base["innerMessageEncoding"] = value;
			}
		}

		public override void ApplyConfiguration(BindingElement bindingElement)
		{
			GZipMessageEncodingBindingElement gZipMessageEncodingBindingElement = (GZipMessageEncodingBindingElement)bindingElement;
			PropertyInformationCollection properties = base.ElementInformation.Properties;
			if (properties["innerMessageEncoding"].ValueOrigin != 0)
			{
				string innerMessageEncoding = InnerMessageEncoding;
				if (!(innerMessageEncoding == "textMessageEncoding"))
				{
					if (innerMessageEncoding == "binaryMessageEncoding")
					{
						gZipMessageEncodingBindingElement.InnerMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
					}
				}
				else
				{
					gZipMessageEncodingBindingElement.InnerMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
				}
			}
		}

		protected override BindingElement CreateBindingElement()
		{
			GZipMessageEncodingBindingElement gZipMessageEncodingBindingElement = new GZipMessageEncodingBindingElement();
			ApplyConfiguration(gZipMessageEncodingBindingElement);
			return gZipMessageEncodingBindingElement;
		}
	}
}
