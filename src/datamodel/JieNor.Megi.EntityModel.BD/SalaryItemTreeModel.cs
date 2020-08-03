using System;
using System.Collections.Generic;

namespace JieNor.Megi.EntityModel.BD
{
	public class SalaryItemTreeModel
	{
		public string id
		{
			get;
			set;
		}

		public string text
		{
			get;
			set;
		}

		public List<SalaryItemTreeChildModel> children
		{
			get;
			set;
		}

		public string _parentId
		{
			get;
			set;
		}

		public string state
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		public bool MIsActive
		{
			get
			{
				return IsActive;
			}
			set
			{
			}
		}

		public int ItemType
		{
			get;
			set;
		}

		public DateTime MCreateDate
		{
			get;
			set;
		}
	}
}
