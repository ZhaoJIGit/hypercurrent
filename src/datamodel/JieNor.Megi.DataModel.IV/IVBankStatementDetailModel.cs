using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankStatementDetailModel : IVBankStatementViewModel
	{
		[DataMember]
		public int MVoucherStatus
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
		public string MVoucherNumber
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
		public bool MIsCanMark
		{
			get;
			set;
		}

		public IVBankStatementDetailModel()
		{
			MIsCanMark = true;
		}
	}
}
