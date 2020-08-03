using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsTypeModel : BDModel
	{
		[DataMember]
		[ApiMember("ContactGroupID", IsPKField = true, IgnoreLengthValidate = true)]
		public string MContactGroupID
		{
			get
			{
				return base.MItemID;
			}
			set
			{
				base.MItemID = value;
			}
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, false, false)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ApiDetail]
		[ApiMember("Contacts", IgnoreInSubModel = true)]
		public List<BDContactsSimpleModel> MContacts
		{
			get;
			set;
		}

		[DataMember(EmitDefaultValue = true)]
		[ApiMember("ContactID", true, IgnoreInGet = true, IgnoreInSubModel = true)]
		public string MContactID
		{
			get;
			set;
		}

		public BDContactsTypeModel()
			: base("T_BD_ContactsType")
		{
		}
	}
}
