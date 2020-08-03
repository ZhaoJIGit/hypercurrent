using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLDocVoucherModel : BDModel
	{
		[DataMember]
		public int __active__ = 1;

		[DataMember]
		public int MDocType
		{
			get;
			set;
		}

		[DataMember]
		public string MDocID
		{
			get;
			set;
		}

		[DataMember]
		public string MEntryID
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
		public string MDebitEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxEntryID
		{
			get;
			set;
		}

		[DataMember]
		public string MCreditEntryID
		{
			get;
			set;
		}

		[DataMember]
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public int MergeStatus
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsActive
		{
			get
			{
				return __active__ == 1;
			}
			set
			{
				__active__ = (value ? 1 : (-1));
			}
		}

		public GLDocVoucherModel(string tableName)
			: base(tableName)
		{
		}

		public GLDocVoucherModel()
			: base("T_GL_Doc_Voucher")
		{
		}
	}
}
