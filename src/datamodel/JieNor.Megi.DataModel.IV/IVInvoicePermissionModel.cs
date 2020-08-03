using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVInvoicePermissionModel
	{
		[DataMember]
		public bool MIsCanPay
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

		[DataMember]
		public bool MIsApprove
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
		public bool MIsCanUnApprove
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
		public bool MIsCanCreateCreditNote
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanCreateRepeatingInvoice
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanEdit
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsInitCanEdit
		{
			get;
			set;
		}

		public bool MHaveAction
		{
			get
			{
				return MIsCanPay || MIsCanVerification || MIsCanApprove || MIsCanUnApprove || MIsCanViewVoucherCreateDetail || MIsCanCopy || MIsCanVoid || MIsCanDelete || MIsCanCreateCreditNote || MIsCanCreateRepeatingInvoice;
			}
			set
			{
				MIsCanPay = value;
				MIsCanVerification = value;
				MIsCanApprove = value;
				MIsCanUnApprove = value;
				MIsCanViewVoucherCreateDetail = value;
				MIsCanCopy = value;
				MIsCanVoid = false;
				MIsCanDelete = value;
				MIsCanCreateCreditNote = value;
				MIsCanCreateRepeatingInvoice = value;
				MIsCanDelete = value;
				MIsCanEdit = value;
				MIsInitCanEdit = value;
			}
		}
	}
}
