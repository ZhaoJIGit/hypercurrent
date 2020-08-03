using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASEnumMappingModel : BDModel
	{
		[DataMember]
		public string MType
		{
			get;
			set;
		}

		[DataMember]
		public string MPublicValue
		{
			get;
			set;
		}

		[DataMember]
		public string MInternalValue
		{
			get;
			set;
		}

		public BASEnumMappingModel()
			: base("T_BAS_EnumMapping")
		{
		}
	}
}
