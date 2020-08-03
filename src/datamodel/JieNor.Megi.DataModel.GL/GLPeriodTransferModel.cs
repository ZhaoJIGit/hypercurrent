using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLPeriodTransferModel : BDModel
	{
		[DataMember]
		public string MVoucherID
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
		public int MVoucherStatus
		{
			get;
			set;
		}

		[DataMember]
		public int MTransferTypeID
		{
			get;
			set;
		}

		[DataMember]
		public string MTransferTypeName
		{
			get;
			set;
		}

		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		[DataMember]
		public bool MNotNeedCreateVoucher
		{
			get;
			set;
		}

		[DataMember]
		public bool MNeedEdit
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPercent0
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPercent1
		{
			get;
			set;
		}

		[DataMember]
		public decimal MPercent2
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCalculateMatch
		{
			get;
			set;
		}

		[DataMember]
		public string MErrorMessage
		{
			get;
			set;
		}

		[DataMember]
		public List<NameValueModel> MNameValueModels
		{
			get;
			set;
		}

		public GLPeriodTransferModel()
			: base("T_GL_PeriodTransfer")
		{
		}

		public GLPeriodTransferModel(string tableName)
			: base(tableName)
		{
		}
	}
}
