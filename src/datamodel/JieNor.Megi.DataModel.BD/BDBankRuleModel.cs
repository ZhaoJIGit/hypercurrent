using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankRuleModel : BDModel
	{
		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		public bool MChkAmount
		{
			get;
			set;
		}

		[DataMember]
		public bool MChkPayee
		{
			get;
			set;
		}

		[DataMember]
		public bool MChkRef
		{
			get;
			set;
		}

		[DataMember]
		public bool MChkTransDate
		{
			get;
			set;
		}

		public BDBankRuleModel()
			: base("T_BD_BankRule")
		{
		}
	}
}
