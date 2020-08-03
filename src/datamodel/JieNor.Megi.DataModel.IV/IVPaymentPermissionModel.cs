using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVPaymentPermissionModel
	{
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

		[DataMember]
		public bool MIsCanViewVoucherCreateDetail
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
		public bool MIsCanViewReconcile
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsCanDeleteReconcile
		{
			get;
			set;
		}

		[DataMember]
		public bool MMarkAsReconciled
		{
			get;
			set;
		}

		[DataMember]
		public bool MMarkAsUnReconciled
		{
			get;
			set;
		}

		[DataMember]
		public bool MHaveAction
		{
			get
			{
				return MIsCanEdit || MIsInitCanEdit || MIsCanVerification || MIsCanViewVoucherCreateDetail || MIsCanVoid || MIsCanDelete || MIsCanViewReconcile || MIsCanDeleteReconcile || MMarkAsReconciled || MMarkAsUnReconciled;
			}
			set
			{
				MIsCanVoid = false;
				MIsCanDelete = value;
				MIsCanEdit = value;
				MIsInitCanEdit = value;
				MIsCanViewVoucherCreateDetail = value;
				MIsCanVerification = value;
				MIsCanViewReconcile = value;
				MIsCanDeleteReconcile = value;
				MMarkAsReconciled = value;
				MMarkAsUnReconciled = value;
			}
		}
	}
}
