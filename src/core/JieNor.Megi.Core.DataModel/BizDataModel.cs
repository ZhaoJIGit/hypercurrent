using MongoDB.Bson;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BizDataModel : BaseModel
	{
		public override string PKFieldName
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
				return MID;
			}
		}

		[DataMember(Order = 1)]
		public string MID
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
		public string MCurrentAccountCode
		{
			get;
			set;
		}

		[DataMember]
		public string MVoucherID
		{
			get;
			set;
		}

		[DataMember]
		public bool MGeneratedNewVoucher
		{
			get;
			set;
		}

		public bool? IncludeDetail
		{
			get;
			set;
		}

		public BizDataModel(string tableName)
			: base(tableName)
		{
		}
	}
}
