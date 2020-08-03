using JieNor.Megi.Core.DataModel;
using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.GL
{
	[DataContract]
	public class GLBalanceQtyModel : BDModel
	{
		[DataMember]
		public string MAccountID
		{
			get;
			set;
		}

		public DateTime MDate
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

		[DataMember]
		public string MCurrencyID
		{
			get;
			set;
		}

		[DataMember]
		public decimal MBeginQty
		{
			get;
			set;
		}

		[DataMember]
		public decimal MDebitQty
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdDebitQty
		{
			get;
			set;
		}

		[DataMember]
		public decimal MYtdCreditQty
		{
			get;
			set;
		}

		[DataMember]
		public decimal MEndQty
		{
			get;
			set;
		}

		[DataMember]
		public int MAdjustPeriod
		{
			get;
			set;
		}

		[DataMember]
		public int MYearPeriod
		{
			get;
			set;
		}

		public GLBalanceQtyModel()
			: base("T_GL_BALANCEQTY")
		{
		}

		public GLBalanceQtyModel(string tableName)
			: base(tableName)
		{
		}
	}
}
