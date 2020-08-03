using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankTypeModel : BDModel
	{
		[DataMember]
		[ApiMember("BankTypeID", IsPKField = true)]
		public string MBankTypeID
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
		[ApiMember("BankTypeName", ApiMemberType.MultiLang, false, false)]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("IsSystem")]
		public bool MIsSys
		{
			get;
			set;
		}

		[DataMember]
		public string MLocaleID
		{
			get;
			set;
		}

		[DataMember]
		public string Message
		{
			get;
			set;
		}

		public BDBankTypeModel()
			: base("T_BD_BankType")
		{
		}

		public BDBankTypeModel(string tableName)
			: base(tableName)
		{
		}
	}
}
