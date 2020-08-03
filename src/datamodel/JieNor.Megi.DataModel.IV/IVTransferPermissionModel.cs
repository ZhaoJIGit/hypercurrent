using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class IVTransferPermissionModel
	{
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
		public bool MIsCanViewVoucherCreateDetail
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
				return MIsCanEdit || MIsCanDelete || MIsCanViewVoucherCreateDetail || MIsCanViewReconcile || MIsCanDeleteReconcile || MMarkAsReconciled || MMarkAsUnReconciled;
			}
			set
			{
				MIsCanDelete = value;
				MIsCanEdit = value;
				MIsCanViewVoucherCreateDetail = value;
				MIsCanViewReconcile = value;
				MIsCanDeleteReconcile = value;
				MMarkAsReconciled = value;
				MMarkAsUnReconciled = value;
			}
		}
	}
}
