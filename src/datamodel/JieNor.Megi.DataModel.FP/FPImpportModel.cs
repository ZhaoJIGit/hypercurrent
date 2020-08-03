using JieNor.Megi.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPImpportModel : BizDataModel
	{
		[DataMember]
		public DateTime MDate
		{
			get;
			set;
		}

		[DataMember]
		public int MCount
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MStartDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndDate
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
		public decimal MTaxAmount
		{
			get;
			set;
		}

		[DataMember]
		public decimal MTotalAmount
		{
			get;
			set;
		}

		[DataMember]
		public string MFileName
		{
			get;
			set;
		}

		[DataMember]
		public int MSource
		{
			get;
			set;
		}

		[DataMember]
		public int MFapiaoCategory
		{
			get;
			set;
		}

		[DataMember]
		public string MOperator
		{
			get;
			set;
		}

		[DataMember]
		public List<FPFapiaoModel> MFapiaoList
		{
			get;
			set;
		}

		public FPImpportModel()
			: base("T_FP_Import")
		{
		}

		public FPImpportModel(string tableName)
			: base(tableName)
		{
		}
	}
}
