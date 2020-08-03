using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPConfigSettingModel : BDModel
	{
		[DataMember]
		public string MID
		{
			get;
			set;
		}

		[DataMember]
		public int MType
		{
			get;
			set;
		}

		[DataMember]
		public bool MValue
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsKey
		{
			get;
			set;
		}

		public FPConfigSettingModel()
			: base("T_FP_ConfigSetting")
		{
		}
	}
}
