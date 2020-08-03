using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDContactsSimpleModel
	{
		[DataMember]
		[ApiMember("ContactID", IsPKField = true)]
		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("Name", ApiMemberType.MultiLang, true, false)]
		[ColumnEncrypt]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MModifyDate", IgnoreInGet = true)]
		public DateTime MModifyDate
		{
			get;
			set;
		}
	}
}
