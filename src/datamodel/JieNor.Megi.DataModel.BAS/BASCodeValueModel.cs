using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASCodeValueModel : BDEntryModel
	{
		[DataMember]
		public string MCodeTableID
		{
			get;
			set;
		}

		[DataMember]
		public string MName
		{
			get;
			set;
		}

		public BASCodeValueModel()
			: base("T_Bas_CodeValue")
		{
		}
	}
}
