using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PAPayRunListModel : PASalaryListBaseModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MDate
		{
			get;
			set;
		}

		[DataMember]
		public string MFormatDate
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
	}
}
