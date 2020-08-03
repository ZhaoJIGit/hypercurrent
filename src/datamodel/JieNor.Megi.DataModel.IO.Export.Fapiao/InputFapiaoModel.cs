using System.ComponentModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Fapiao
{
	[DataContract]
	public class InputFapiaoModel : FapiaoBaseModel
	{
		[DataMember]
		public string SalesEnterpriseTitle
		{
			get;
			set;
		}

		[DataMember]
		public string SalesEnterpriseTaxCodeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string StatusTitle
		{
			get;
			set;
		}

		[DataMember]
		public string VerifyStatusTitle
		{
			get;
			set;
		}

		[DataMember]
		public string VerifyTypeTitle
		{
			get;
			set;
		}

		[DataMember]
		public string VerifyPeriodTitle
		{
			get;
			set;
		}

		[DisplayName("InputFapiao List Rows")]
		[DataMember]
		public InputFapiaoListRowCollection ListRows
		{
			get;
			set;
		}
	}
}
