using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsTypeLinkModel : BDModel
	{
		[DataMember]
		[ApiMember("TypeID")]
		public string MTypeID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("ContactID")]
		public string MContactID
		{
			get;
			set;
		}

		public BDContactsTypeLinkModel()
			: base("T_BD_ContactsTypeLink")
		{
		}
	}
}
