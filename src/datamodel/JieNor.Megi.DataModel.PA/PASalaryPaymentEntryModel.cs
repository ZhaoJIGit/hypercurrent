using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Enum;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.PA
{
	[DataContract]
	public class PASalaryPaymentEntryModel : BizEntryDataModel
	{
		[DataMember]
		public string MPayItemID
		{
			get;
			set;
		}

		[DataMember]
		public string MParentPayItemID
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

		[DataMember]
		public int MCoefficient
		{
			get;
			set;
		}

		[DataMember]
		public string MDesc
		{
			get;
			set;
		}

		[DataMember]
		public PayrollItemEnum MItemType
		{
			get;
			set;
		}

		[DataMember]
		public string MItemName
		{
			get;
			set;
		}

		[DataMember]
		public bool MIsAvailable
		{
			get;
			set;
		}

		public PASalaryPaymentEntryModel()
			: base("T_PA_SalaryPaymentEntry")
		{
		}
	}
}
