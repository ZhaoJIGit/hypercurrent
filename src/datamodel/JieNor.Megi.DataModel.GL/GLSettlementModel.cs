using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLSettlementModel : BDModel
	{
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
		public int MStatus
		{
			get;
			set;
		}

		[DataMember]
		public string MSettlerID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MSettleDate
		{
			get;
			set;
		}

		[DataMember]
		public bool HasUnapprovedVoucher
		{
			get;
			set;
		}

		[DataMember]
		public bool IsNumberBroken
		{
			get;
			set;
		}

		[DataMember]
		public List<GLUnsettlementModel> UnsettledPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int CurrentPeriod
		{
			get;
			set;
		}

		public GLSettlementModel()
			: base("T_GL_SETTLEMENT")
		{
		}

		public GLSettlementModel(string tableName)
			: base(tableName)
		{
		}
	}
}
