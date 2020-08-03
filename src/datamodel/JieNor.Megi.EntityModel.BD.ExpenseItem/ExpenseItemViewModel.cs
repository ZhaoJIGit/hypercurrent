using System;

namespace JieNor.Megi.EntityModel.BD.ExpenseItem
{
	public class ExpenseItemViewModel
	{
		public string MParentItemID
		{
			get;
			set;
		}

		public string MItemID
		{
			get;
			set;
		}

		public bool MIsIncludeAccount
		{
			get;
			set;
		}

		public string MName
		{
			get;
			set;
		}

		public string MParentName
		{
			get;
			set;
		}

		public string MDesc
		{
			get;
			set;
		}

		public string MexpenseAccountID
		{
			get;
			set;
		}

		public string MText
		{
			get
			{
				return MName + ":" + MDesc;
			}
			set
			{
			}
		}

		public DateTime MModifyDate
		{
			get;
			set;
		}
	}
}
