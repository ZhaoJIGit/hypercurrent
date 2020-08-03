using System;
using System.Collections.Specialized;

namespace JieNor.Megi.Core
{
	public class ApiGetParam : ApiParam
	{
		public string ElementID
		{
			get;
			set;
		}

		public DateTime ModifiedSince
		{
			get;
			set;
		}

		public string WhereString
		{
			get;
			set;
		}

		public string Where
		{
			get;
			set;
		}

		public string OrderBy
		{
			get;
			set;
		}

		public int Page
		{
			get;
			set;
		}

		public string IncludeDisabled
		{
			get;
			set;
		}

		public string IncludeDetail
		{
			get;
			set;
		}

		public NameValueCollection QueryString
		{
			get;
			set;
		}
	}
}
