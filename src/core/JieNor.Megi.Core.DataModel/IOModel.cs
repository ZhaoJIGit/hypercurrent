using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class IOModel : BaseModel
	{
		public override string PKFieldName
		{
			get
			{
				return "MItemID";
			}
		}

		public override string PKFieldValue
		{
			get
			{
				return MItemID;
			}
		}

		[DataMember]
		public string MItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
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

		public IOModel(string tableName)
			: base(tableName)
		{
		}
	}
}
