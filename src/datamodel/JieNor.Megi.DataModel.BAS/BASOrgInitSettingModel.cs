using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrgInitSettingModel : BDModel
	{
		[DataMember]
		public DateTime MConversionDate
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountingStandard
		{
			get;
			set;
		}

		public BASOrgInitSettingModel()
			: base("T_Bas_OrgInitSetting")
		{
		}

		public BASOrgInitSettingModel(string tableName)
			: base(tableName)
		{
		}
	}
}
