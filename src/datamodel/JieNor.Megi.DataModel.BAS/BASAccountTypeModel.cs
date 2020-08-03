using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASAccountTypeModel : BDModel
	{
		[DataMember]
		public string MAccountGroupID
		{
			get;
			set;
		}

		[DataMember]
		public string MAccountTableID
		{
			get;
			set;
		}

		[DataMember]
		public int MDC
		{
			get;
			set;
		}

		[DataMember]
		public string MParentID
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

		public BASAccountTypeModel()
			: base("T_BAS_ACCOUNTTYPE")
		{
		}

		public BASAccountTypeModel(string tableName)
			: base(tableName)
		{
		}
	}
}
