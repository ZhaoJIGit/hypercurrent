using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Xml;

namespace JieNor.Megi.GZipEncoder
{
	public class GZipMessageEncodingBindingElementImporter : IPolicyImportExtension
	{
		void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
		{
			if (importer == null)
			{
				throw new ArgumentNullException("importer");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			ICollection<XmlElement> bindingAssertions = context.GetBindingAssertions();
			foreach (XmlElement item in bindingAssertions)
			{
				if (item.NamespaceURI == "http://schemas.microsoft.com/ws/06/2004/mspolicy/netgzip1" && item.LocalName == "GZipEncoding")
				{
					bindingAssertions.Remove(item);
					context.BindingElements.Add(new GZipMessageEncodingBindingElement());
					break;
				}
			}
		}
	}
}
