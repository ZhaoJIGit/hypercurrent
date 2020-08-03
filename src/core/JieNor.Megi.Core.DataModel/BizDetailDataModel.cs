using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizDetailDataModel : BaseModel
	{
		public override string PKFieldName
		{
			get
			{
				return "MDetailID";
			}
		}

		public override string PKFieldValue
		{
			get
			{
				return MDetailID;
			}
		}

		[DataMember]
		public string MDetailID
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryID
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

		public BizDetailDataModel(string tableName)
			: base(tableName)
		{
		}
	}
}
