using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASOrgModuleModel : BDModel
	{
		[DataMember]
		public string MModuleID
		{
			get;
			set;
		}

		[DataMember]
		public DateTime MExpiredDate
		{
			get;
			set;
		}

		public BASOrgModuleModel()
			: base("T_Bas_OrgModule")
		{
		}
	}
}
