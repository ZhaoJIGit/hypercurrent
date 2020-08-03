using JieNor.Megi.Core.Attribute;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizEntryDataModel : BaseModel
	{
		public override string PKFieldName
		{
			get
			{
				return "MEntryID";
			}
		}

		public override string FKFieldName
		{
			get
			{
				return "MID";
			}
		}

		public override string PKFieldValue
		{
			get
			{
				return MEntryID;
			}
		}

		[DataMember]
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		[ApiMember("MSeq", IgnoreInGet = true)]
		public int MSeq
		{
			get;
			set;
		}

		[DataMember]
		public string MDebitAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditAccount
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxAccount
		{
			get;
			set;
		}

		public BizEntryDataModel(string tableName)
			: base(tableName)
		{
		}
	}
}
