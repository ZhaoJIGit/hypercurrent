using JieNor.Megi.Core.DataModel;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLBalanceProfitModel : BDModel
	{
		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		[DataMember]
		public int MYear
		{
			get;
			set;
		}

		[DataMember]
		public int MPeriod
		{
			get;
			set;
		}

		public string MContactID
		{
			get;
			set;
		}

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MAmountFor
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
		public decimal MYtdAmountFor
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdAmount
		{
			get;
			set;
		}

		public GLBalanceProfitModel()
			: base("T_GL_BALANCEQTY")
		{
		}

		public GLBalanceProfitModel(string tableName)
			: base(tableName)
		{
		}
	}
}
