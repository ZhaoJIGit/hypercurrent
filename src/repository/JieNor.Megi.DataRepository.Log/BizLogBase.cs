using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.Log
{
	public class BizLogBase
	{
		public void AddLog(OptLogTemplate template, MContext ctx, string pkId, params object[] formatValues)
		{
			OptLog.AddLog(template, ctx, pkId, formatValues);
		}

		public CommandInfo GetLogCmd(OptLogTemplate template, MContext ctx, string pkId, bool createDateAdd, params object[] formatValues)
		{
			return OptLog.GetAddLogCommand(template, ctx, pkId, createDateAdd, formatValues);
		}

		public CommandInfo GetLogCmdList(OptLogTemplate template, MContext ctx, List<string> pkIdList, bool createDateAdd, Dictionary<string, List<object>> formatValues)
		{
			return OptLog.GetAddLogCommand(template, ctx, pkIdList, createDateAdd, formatValues);
		}
	}
}
