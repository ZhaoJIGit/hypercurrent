using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.BAS
{
	[DataContract]
	public class BASAccountTableModel : BDModel
	{
		public string MAccoutGroupID
		{
			get;
			set;
		}

		public BASAccountTableModel()
			: base("T_BAS_ACCOUNTTABLE")
		{
		}

		public BASAccountTableModel(string tableName)
			: base(tableName)
		{
		}
	}
}
