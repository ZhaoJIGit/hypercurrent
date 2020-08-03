using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVExpensePermissionModel
	{
		[DataMember]
		public bool MIsCanPay
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanUnApprove
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanApprove
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanViewVoucherCreateDetail
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanCopy
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanVoid
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanDelete
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanVerification
		{
			get;
			set;
		}

		public bool MHaveAction
		{
			get
			{
				return MIsCanPay || MIsCanApprove || MIsCanUnApprove || MIsCanViewVoucherCreateDetail || MIsCanCopy || MIsCanVoid || MIsCanDelete || MIsCanVerification;
			}
			set
			{
				MIsCanPay = value;
				MIsCanApprove = value;
				MIsCanUnApprove = value;
				MIsCanViewVoucherCreateDetail = value;
				MIsCanCopy = value;
				MIsCanVoid = false;
				MIsCanDelete = value;
				MIsCanVerification = value;
			}
		}
	}
}
