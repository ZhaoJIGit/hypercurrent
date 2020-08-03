using System.Runtime.Serialization;

namespace JieNor.Megi.DataModel.IO.Export.Account
{
	[DataContract]
	public class AccountModel : ExportBaseModel
	{
		public string CodeTitle
		{
			get;
			set;
		}

		public string NameTitle
		{
			get;
			set;
		}

		public string GroupTitle
		{
			get;
			set;
		}

		public string TypeTitle
		{
			get;
			set;
		}

		public string DirectionTitle
		{
			get;
			set;
		}

		public string AccountStandardTitle
		{
			get;
			set;
		}

		public string IsSysTitle
		{
			get;
			set;
		}

		public string IsCheckForCurrencyTitle
		{
			get;
			set;
		}

		public string CheckGroupTitle
		{
			get;
			set;
		}

		public AccountRowCollection AccountRows
		{
			get;
			set;
		}
	}
}
