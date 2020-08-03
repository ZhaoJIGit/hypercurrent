using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASVoucherGroupModel : BDModel
	{
		[DataMember]
		public bool MIsDefault
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		[DataMember]
		public string MAcctTableID
		{
			get;
			set;
		}

		public BASVoucherGroupModel()
			: base("T_BAS_VOUCHERGROUP")
		{
		}

		public BASVoucherGroupModel(string tableName)
			: base(tableName)
		{
		}
	}
}
