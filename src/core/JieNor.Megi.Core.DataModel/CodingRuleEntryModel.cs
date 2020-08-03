using JieNor.Megi.EntityModel.Context;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core.DataModel
{
	[DataContract]
	public class CodingRuleEntryModel : BizEntryDataModel
	{
		public string MDataType
		{
			get;
			set;
		}

		public string MValue
		{
			get;
			set;
		}

		public string MFieldName
		{
			get;
			set;
		}

		public string MFormat
		{
			get;
			set;
		}

		[DataMember]
		public int MSerialDigit
		{
			get;
			set;
		}

		[DataMember]
		public int MSerialStart
		{
			get;
			set;
		}

		public MContext MContext
		{
			get;
			set;
		}

		public CodingRuleEntryModel()
			: base("T_Bas_CodingRuleEntry")
		{
		}
	}
}
