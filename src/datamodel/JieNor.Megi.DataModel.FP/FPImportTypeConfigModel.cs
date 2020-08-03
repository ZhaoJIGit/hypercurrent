using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPImportTypeConfigModel : BDModel
	{
		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public int MFPType
		{
			get;
			set;
		}

		[DataMember]
		public int MImportType
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountNo
		{
			get;
			set;
		}

		[DataMember]
		public string MPassword
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MLastUploadDate
		{
			get;
			set;
		}

		[DataMember]
		public int MMonthAgo
		{
			get;
			set;
		}

		public FPImportTypeConfigModel()
			: base("T_FP_ImportTypeConfig")
		{
		}
	}
}
