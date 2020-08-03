using System;
using System.Collections.Specialized;

namespace JieNor.Megi.EntityModel
{
	public class GetParam : ExParamBase
	{
		public DateTime ModifiedSince
		{
			get;
			set;
		}

		public string Where
		{
			get;
			set;
		}

		public string WhereString
		{
			get;
			set;
		}

		public NameValueCollection QueryString
		{
			get;
			set;
		}

		public string OrderBy
		{
			get;
			set;
		}

		public int PageIndex
		{
			get;
			set;
		}

		public int PageSize
		{
			get;
			set;
		}

		public bool IncludeDisabled
		{
			get;
			set;
		}

		public bool? IncludeDetail
		{
			get;
			set;
		}

		public string PostData
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public string Tag
		{
			get;
			set;
		}

		public bool FromPost
		{
			get;
			set;
		}

		public GetParam()
		{
			PageIndex = 1;
			PageSize = 0;
		}
	}
}
