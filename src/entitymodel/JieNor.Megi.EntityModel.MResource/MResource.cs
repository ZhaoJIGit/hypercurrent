using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.MResource
{
	[DataContract]
	public class MResource
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public string MUserID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccessToken
		{
			get;
			set;
		}

		[DataMember]
		public string MOrgID
		{
			get;
			set;
		}

		[DataMember]
		public string MTableName
		{
			get;
			set;
		}

		[DataMember]
		public string MPrefix
		{
			get;
			set;
		}

		[DataMember]
		public string MResourcePrefix
		{
			get;
			set;
		}

		[DataMember]
		public string MField
		{
			get;
			set;
		}

		[DataMember]
		public object MFieldValue
		{
			get;
			set;
		}

		[DataMember]
		public string MPKFieldValue
		{
			get;
			set;
		}

		[DataMember]
		public string MPKFieldName
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MCreateDate
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsDelete
		{
			get;
			set;
		}
	}
}
