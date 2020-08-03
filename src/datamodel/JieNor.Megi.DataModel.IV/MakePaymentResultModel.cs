using JieNor.Megi.Core.DBUtility;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	[DataContract]
	public class MakePaymentResultModel
	{
		[DataMember]
		public List<CommandInfo> MCommand
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetBillType
		{
			get;
			set;
		}

		[DataMember]
		public string MTargetBillID
		{
			get;
			set;
		}

		[DataMember]
		public bool MSuccess
		{
			get;
			set;
		}

		[DataMember]
		public string MMessage
		{
			get;
			set;
		}
	}
}
