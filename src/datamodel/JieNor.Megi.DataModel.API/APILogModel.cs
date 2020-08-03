using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.API
{
	[DataContract]
	public class APILogModel : BizDataModel
	{
		[DataMember]
		public string MConsumerKey
		{
			get;
			set;
		}

		[DataMember]
		public string MProviderName
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
		public string MIP
		{
			get;
			set;
		}

		[DataMember]
		public string MModule
		{
			get;
			set;
		}

		[DataMember]
		public string MUrl
		{
			get;
			set;
		}

		[DataMember]
		public string MMethod
		{
			get;
			set;
		}

		[DataMember]
		public string MHeader
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
		public string MPostData
		{
			get;
			set;
		}

		[DataMember]
		public string MResponseData
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MBeginExecuteDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MEndExecuteDate
		{
			get;
			set;
		}

		[DataMember]
		public int MExecuteTime
		{
			get;
			set;
		}

		[DataMember]
		public string MStatus
		{
			get;
			set;
		}

		public APILogModel()
			: base("t_app_log")
		{
		}
	}
}
