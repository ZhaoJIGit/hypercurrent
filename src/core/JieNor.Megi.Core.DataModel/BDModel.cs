using MongoDB.Bson;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BDModel : BaseModel
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
			set
			{
				MItemID = value;
			}
		}

		[DataMember(Order = 1)]
		public string MItemID
		{
			get;
			set;
		}

		public ObjectId _id
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

		
		[DataMember(Order = 2, EmitDefaultValue = true)]
		public string MNumber
		{
			get;
			set;
		}

		[DataMember]
		public int MRowIndex
		{
			get;
			set;
		}

		public BDModel(string tableName)
			: base(tableName)
		{
		}
	}
}
