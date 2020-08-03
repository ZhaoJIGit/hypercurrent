using JieNor.Megi.Core;
using System.Collections.Generic;

namespace JieNor.Megi.DataModel.GL
{
	public class GLBalanceAccountTreeModel
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

		public string MNumber
		{
			get;
			set;
		}

		public List<GLBalanceAccountTreeModel> children
		{
			get;
			set;
		}

		public string AccountID
		{
			get;
			set;
		}

		public bool IsCheckTypeAccount
		{
			get;
			set;
		}

		public List<NameValueModel> CheckTypeValueList
		{
			get;
			set;
		}

		public string CheckGroupValueId
		{
			get;
			set;
		}
	}
}
