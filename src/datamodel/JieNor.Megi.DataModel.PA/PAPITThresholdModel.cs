using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPITThresholdModel : BDModel
	{
		[DataMember]
		public string MEmployeeID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEffectiveDate
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
		public decimal MDefaultAmount
		{
			get;
			set;
		}

		public PAPITThresholdModel()
			: base("t_pa_pitthreshold")
		{
		}
	}
}
