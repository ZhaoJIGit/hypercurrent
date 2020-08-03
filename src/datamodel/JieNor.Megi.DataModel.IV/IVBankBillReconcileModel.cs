using JieNor.Megi.Core.Attribute;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVBankBillReconcileModel : BizDataModel
	{
		[DataMember]
		public string MBankBillEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetBillType
		{
			get;
			set;
		}

		[DataMember]
		public decimal MSpentAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MReceiveAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAdjustAmtFor
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[ModelEntry]
		[ApiDetail]
		[DataMember]
		public List<IVBankBillReconcileEntryModel> RecEntryList
		{
			get;
			set;
		}

		public IVBankBillReconcileModel()
			: base("T_IV_BankBillReconcile")
		{
			RecEntryList = new List<IVBankBillReconcileEntryModel>();
		}
	}
}
