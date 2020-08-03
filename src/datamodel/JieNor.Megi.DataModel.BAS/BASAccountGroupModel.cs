using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASAccountGroupModel : BDModel
	{
		public int MDC
		{
			get;
			set;
		}

		public string MAccountType
		{
			get;
			set;
		}

		public BASAccountGroupModel()
			: base("T_BAS_CCOUNTGROUP")
		{
		}

		public BASAccountGroupModel(string tableName)
			: base(tableName)
		{
		}
	}
}
