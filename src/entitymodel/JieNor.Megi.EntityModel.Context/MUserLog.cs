using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.EntityModel.Context
{
	[DataContract]
	public class MUserLog
	{
		[DataMember]
		public string Id
		{
			get;
			set;
		}

		[DataMember]
		public string MEmail
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
		public string MPath
		{
			get;
			set;
		}

		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string MUserAddress
		{
			get;
			set;
		}

		[DataMember]
		public string MParameters
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
		public string MLocaleID
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
		public string MNavigator
		{
			get;
			set;
		}

		[DataMember]
		public string MTabTitle
		{
			get;
			set;
		}
	}
}
