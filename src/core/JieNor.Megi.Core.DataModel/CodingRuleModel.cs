using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class CodingRuleModel : BizDataModel
	{
		public string MObjectType
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsDefault
		{
			get;
			set;
		}

		public List<CodingRuleEntryModel> CodingRuleEntries
		{
			get;
			set;
		}

		public MContext MContext
		{
			get;
			set;
		}

		public CodingRuleModel()
			: base("T_Bas_CodingRule")
		{
		}
	}
}
