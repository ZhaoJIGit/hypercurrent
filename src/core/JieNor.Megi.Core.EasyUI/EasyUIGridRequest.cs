using System;
using System.Web;

namespace JieNor.Megi.Core.EasyUI
{
	public class EasyUIGridRequest
	{
		private int _PageNumber;

		private int _PageSize;

		public string SortName
		{
			get;
			set;
		}

		public bool SortOrder
		{
			get;
			set;
		}

		public int PageNumber
		{
			get
			{
				if (_PageNumber <= 0)
				{
					return 1;
				}
				return _PageNumber;
			}
			set
			{
				if (value <= 0)
				{
					_PageNumber = 1;
				}
				else
				{
					_PageNumber = value;
				}
			}
		}

		public int PageSize
		{
			get
			{
				return _PageSize;
			}
			set
			{
				_PageSize = ((value == 0) ? 10 : value);
			}
		}

		public string Where
		{
			get;
			set;
		}

		public EasyUIGridRequest(HttpContextBase context)
		{
			SortName = context.Request.Form["sort"];
			SortOrder = (context.Request.Form["order"] == "desc");
			_PageNumber = Convert.ToInt32(context.Request.Form["page"]);
			PageSize = Convert.ToInt32(context.Request.Form["rows"]);
		}
	}
}
