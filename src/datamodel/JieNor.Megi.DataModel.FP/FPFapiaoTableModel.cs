using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.FP
{
	[DataContract]
	public class FPFapiaoTableModel : BDModel
	{
		[DataMember]
		public string MFapiaoID
		{
			get;
			set;
		}

		[DataMember]
		public string MTableID
		{
			get;
			set;
		}

		[DataMember]
		public int MFapiaoType
		{
			get;
			set;
		}

		public FPFapiaoTableModel()
			: base("T_FP_Fapiao_Table")
		{
		}

		public FPFapiaoTableModel(string tableName)
			: base(tableName)
		{
		}
	}
}
