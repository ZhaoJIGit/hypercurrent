using JieNor.Megi.Core;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryListFilterModel : SqlWhere
	{
		[DataMember]
		public string MPrintSettingID
		{
			get;
			set;
		}

		[DataMember]
		public string ObjectIds
		{
			get;
			set;
		}

		[DataMember]
		public DateTime SalaryMonth
		{
			get;
			set;
		}

		[DataMember]
		public string MRunID
		{
			get;
			set;
		}

		[DataMember]
		public PAPrintSettingModel PrintSettingModel
		{
			get;
			set;
		}
	}
}
