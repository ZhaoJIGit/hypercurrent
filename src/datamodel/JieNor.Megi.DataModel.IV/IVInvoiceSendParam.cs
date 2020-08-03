using JieNor.Megi.Core.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IV
{
	public class IVInvoiceSendParam : ParamBase
	{
		[DataMember]
		public EmailSendTypeEnum SendType
		{
			get;
			set;
		}

		[DataMember]
		public string ContactID
		{
			get;
			set;
		}
	}
}
