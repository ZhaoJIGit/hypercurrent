using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryToIVPayEntryModel : BizEntryDataModel
	{
		[DataMember]
		public string MSalaryPaymentID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmount
		{
			get;
			set;
		}

		public PASalaryToIVPayEntryModel()
			: base("T_PA_SalaryToIVPayEntry")
		{
		}
	}
}
