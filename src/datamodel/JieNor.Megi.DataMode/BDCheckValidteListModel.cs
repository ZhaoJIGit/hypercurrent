using System.Collections.Generic;

namespace JieNor.Megi.DataMode
{
	public class BDCheckValidteListModel
	{
		public List<string> ContactIdList
		{
			get;
			set;
		}

		public List<string> EmployeeIdList
		{
			get;
			set;
		}

		public List<string> MerchandiseIdList
		{
			get;
			set;
		}

		public List<string> ExpenseIdList
		{
			get;
			set;
		}

		public List<string> PaIdList
		{
			get;
			set;
		}

		public List<string> PaGroupIdList
		{
			get;
			set;
		}

		public List<string> TrackEntryIdList
		{
			get;
			set;
		}

		public List<string> AccountIdList
		{
			get;
			set;
		}

		public List<string> BankIdList
		{
			get;
			set;
		}

		public BDCheckValidteListModel()
		{
			ContactIdList = new List<string>();
			EmployeeIdList = new List<string>();
			MerchandiseIdList = new List<string>();
			ExpenseIdList = new List<string>();
			PaIdList = new List<string>();
			PaGroupIdList = new List<string>();
			TrackEntryIdList = new List<string>();
			AccountIdList = new List<string>();
			BankIdList = new List<string>();
		}
	}
}
