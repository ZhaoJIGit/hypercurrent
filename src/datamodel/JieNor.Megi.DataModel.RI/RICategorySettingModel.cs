using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.RI
{
	[DataContract]
	public class RICategorySettingModel : BDModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public bool MEnable
		{
			get;
			set;
		}

		[DataMember]
		public bool MRequirePass
		{
			get;
			set;
		}

		[DataMember]
		public RICategorySettingParamModel MSettingParam
		{
			get;
			set;
		}

		public RICategorySettingModel()
			: base("T_RI_CategorySetting")
		{
		}

		public RICategorySettingModel(string tableName)
			: base(tableName)
		{
		}
	}
}
