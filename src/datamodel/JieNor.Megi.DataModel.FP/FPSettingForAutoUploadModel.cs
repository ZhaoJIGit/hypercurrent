using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPSettingForAutoUploadModel : FPImportTypeConfigModel
	{
		[DataMember]
		public string MOrgName
		{
			get;
			set;
		}

		[DataMember]
		public string MViewBizObjectIDs
		{
			get;
			set;
		}

		[DataMember]
		public string MChangeBizObjectIDs
		{
			get;
			set;
		}

		[DataMember]
		public string MTaxNo
		{
			get;
			set;
		}

		[DataMember]
		public string MFPTypes
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsOrgExpired
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsOrgDelete
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MGLBeginDate
		{
			get;
			set;
		}
	}
}
