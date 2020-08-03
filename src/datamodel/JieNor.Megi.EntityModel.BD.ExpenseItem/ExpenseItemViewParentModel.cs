using System;
using System.Collections.Generic;

namespace JieNor.Megi.EntityModel.BD.ExpenseItem
{
	public class ExpenseItemViewParentModel
	{
		public string id
		{
			get;
			set;
		}

		public string _parentId
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

		public DateTime MModifyDate
		{
			get;
			set;
		}

		public string state
		{
			get;
			set;
		}

		public List<ExpenseItemViewModelBase> children
		{
			get;
			set;
		}
	}
}
