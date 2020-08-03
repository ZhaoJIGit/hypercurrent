using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPayRunModel : BizDataModel
	{
		[DataMember]
		public DateTime MDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MFromDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MToDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MPayDate
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
		public string MSelectIds
		{
			get;
			set;
		}

		public PAPayRunModel()
			: base("T_PA_PayRun")
		{
		}
	}
}
