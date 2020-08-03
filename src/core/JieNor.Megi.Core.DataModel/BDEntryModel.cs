using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BDEntryModel : BaseModel
	{
		public override string PKFieldName
		{
			get
			{
				return "MEntryID";
			}
		}

		public override string PKFieldValue
		{
			get
			{
				return MEntryID;
			}
		}

		[DataMember(Order = 1)]
		public string MEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public int MSeq
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

		public BDEntryModel(string tableName)
			: base(tableName)
		{
		}
	}
}
