using JieNor.Megi.Core.DataModel;

namespace JieNor.Megi.DataModel.BAS
{
	public class BASSettleTypeModel : BaseModel
	{
		public string MItemID
		{
			get;
			set;
		}

		public string MSettleCategory
		{
			get;
			set;
		}

		public string MType
		{
			get;
			set;
		}

		public bool MIsNeedPayCharge
		{
			get;
			set;
		}

		public bool MIsNeedPayCash
		{
			get;
			set;
		}

		public string MPayType
		{
			get;
			set;
		}

		public string MNumber
		{
			get;
			set;
		}

		public BASSettleTypeModel()
			: base("T_BAS_SETTLETYPE")
		{
		}

		public BASSettleTypeModel(string tableName)
			: base(tableName)
		{
		}
	}
}
