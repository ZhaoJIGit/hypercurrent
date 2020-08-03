using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.Core.DBUtility
{
	public class MultiDBCommand
	{
		private string _connString = string.Empty;

		private MContext _ctx = null;

		public SysOrBas DBType
		{
			get;
			set;
		}

		public string ConnectionString
		{
			get
			{
				if (!string.IsNullOrEmpty(_connString))
				{
					return _connString;
				}
				if (DBType == SysOrBas.Sys)
				{
					return PubConstant.ConnectionString;
				}
				return DynamicPubConstant.ConnectionString(_ctx.MOrgID);
			}
			set
			{
				_connString = value;
			}
		}

		public List<CommandInfo> CommandList
		{
			get;
			set;
		}

		public MultiDBCommand(MContext ctx)
		{
			_ctx = ctx;
		}
	}
}
