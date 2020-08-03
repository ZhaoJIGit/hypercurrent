using JieNor.Megi.EntityModel.Context;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class BillNumberModel : BizDataModel
	{
		public string MObjectType
		{
			get;
			set;
		}

		[DataMember]
		public string MRuleID
		{
			get;
			set;
		}

		[DataMember]
		public long MSerial
		{
			get;
			set;
		}

		public MContext MContext
		{
			get;
			set;
		}

		public BillNumberModel()
			: base("t_bas_billnumber")
		{
		}
	}
}
