using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BD
{
	[DataContract]
	public class BDBankBasicModel
	{
		[DataMember]
		public string MBankID
		{
			get;
			set;
		}

		[DataMember]
		[ColumnEncrypt]
		public string MBankNo
		{
			get;
			set;
		}

		[DataMember]
		public string MCyID
		{
			get;
			set;
		}

		[DataMember]
		public string MBankName
		{
			get;
			set;
		}
	}
}
