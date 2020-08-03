using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Fapiao
{
	[DataContract]
	public class OutputFapiaoModel : FapiaoBaseModel
	{
		[DataMember]
		public string PurchasingEnterpriseTitle
		{
			get;
			set;
		}

		[DataMember]
		public string PurchasingEnterpriseTaxCodeTitle
		{
			get;
			set;
		}

		[DisplayName("OutputFapiao List Rows")]
		[DataMember]
		public OutputFapiaoRowCollection ListRows
		{
			get;
			set;
		}
	}
}
